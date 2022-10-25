using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Weather.Client.Helpers;
using Weather.Client.Models;
using Weather.Client.Options;
using Weather.Data;

namespace Weather.Client.HostedServices
{
    /// <summary>
    /// Service to process GRPC
    /// </summary>
    public class WeatherHostedService : BackgroundService
    {
        /// <summary>
        /// Storage of sensors
        /// </summary>
        private readonly ISensorsStorage _storage;

        /// <summary>
        /// Storage of subscriptions to sensors
        /// </summary>
        private readonly ISubscriptionStorage _subscriptions;

        /// <summary>
        /// IServiceProvider
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Service configuration
        /// </summary>
        private readonly ServiceConfig _options;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<WeatherHostedService> _logger;

        /// <summary>
        /// GRPC client
        /// </summary>
        private Emulator.Generator.GeneratorClient client;

        /// <summary>
        /// Cancellation token
        /// </summary>
        private CancellationToken cancellationToken;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="storage"> Storage of sensors </param>
        /// <param name="options"> Service configuration </param>
        /// <param name="provider"> IServiceProvider </param>
        /// <param name="subscriptions"> Storage of subscriptions to sensors </param>
        /// <param name="logger"> Logger </param>
        public WeatherHostedService(ISensorsStorage storage, IOptions<ServiceConfig> options, IServiceProvider provider, ISubscriptionStorage subscriptions, ILogger<WeatherHostedService> logger)
        {
            _storage = storage;
            _provider = provider;
            _subscriptions = subscriptions;
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Service execution (grpc reading)
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token </param>
        /// <returns> Task </returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            await Reconnector.DoWithRetryAsync(ReadGrpcService, _logger, _options.ReconnectionAmount, _options.ReconnectionTimeout);
        }

        /// <summary>
        /// Get a GRPC client
        /// </summary>
        /// <returns> GRPC client </returns>
        private async Task<Emulator.Generator.GeneratorClient> GetClient()
        {
            if (client == null)
            {
                await using var scope = _provider.CreateAsyncScope();
                client = scope.ServiceProvider.GetRequiredService<Emulator.Generator.GeneratorClient>();
            }

            return client;
        }

        /// <summary>
        /// A GRPC service reading
        /// </summary>
        /// <returns> Task </returns>
        private async Task<object> ReadGrpcService()
        {
            const int streamTimeOut = 50;
            var client = await GetClient();
            var eventResponseStream = client.EventStream(new Empty(), cancellationToken: this.cancellationToken);
            var serviceCancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(streamTimeOut));

            while (await eventResponseStream.ResponseStream.MoveNext(this.cancellationToken))
            {
                var responseItem = eventResponseStream.ResponseStream.Current;

                if (_subscriptions.Contains(responseItem.SensorInfo.Id))
                {
                    _storage.AddSample(new SensorSample()
                    {
                        CreatedAt = responseItem.CreatedAt.ToDateTime(),
                        EventId = responseItem.EventId,
                        SensorInfo = new Data.SensorInfo()
                        {
                            Id = responseItem.SensorInfo.Id,
                            Name = responseItem.SensorInfo.Name,
                            SensorType = (Data.SensorTypeEnum)(int)responseItem.SensorInfo.SensorType,
                            CO2 = responseItem.Co2,
                            Humidity = responseItem.Humidity,
                            Temperature = responseItem.Temperature,
                        }
                    }, _options.AggregationIntervalTime);
                    serviceCancellationToken.CancelAfter(streamTimeOut);
                }
            }

            return null;
        }
    }
}
