using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Emulator.Mocks;
using Weather.Emulator.Options;

namespace Weather.Emulator.GrpcServices
{
    /// <summary>
    /// GRPC service
    /// </summary>
    public class GeneratorService : Generator.GeneratorBase
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<GeneratorService> _logger;

        /// <summary>
        /// List of sensors
        /// </summary>
        private readonly List<Data.SensorInfo> _sensors;

        /// <summary>
        /// Settings
        /// </summary>
        private readonly ServiceConfig _options;

        /// <summary>
        /// Contructor with parameters
        /// </summary>
        /// <param name="logger"> Logger </param>
        /// <param name="sensors"> List of sensors </param>
        /// <param name="options"> Service settings </param>
        public GeneratorService(ILogger<GeneratorService> logger, IOptions<List<Data.SensorInfo>> sensors, IOptions<ServiceConfig> options)
        {
            _logger = logger;
            _sensors = sensors.Value;
            _options = options.Value;

            ValidateSettings();
        }

        /// <summary>
        /// Settings validate
        /// </summary>
        private void ValidateSettings()
        {
            _options.AskTime = Math.Clamp(_options.AskTime, 100, 2000);
        }

        /// <summary>
        /// Get list of available sensors
        /// </summary>
        /// <param name="request"> Request </param>
        /// <param name="context"> Context </param>
        /// <returns> List of sensors </returns>
        public override Task<GetSensorsResponse> GetSensors(GetSensorsRequest request, ServerCallContext context)
        {
            var result = new GetSensorsResponse();

            foreach (var sensor in _sensors)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                result.Result.Add(new SensorInfo() { Id = sensor.Id, Name = sensor.Name, SensorType = (SensorTypeEnum)(int)sensor.SensorType });
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// GRPC stream
        /// </summary>
        /// <param name="request"> Request </param>
        /// <param name="responseStream"> Stream </param>
        /// <param name="context"> Context </param>
        /// <returns> Task </returns>
        public override async Task EventStream(Empty request, IServerStreamWriter<EventResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var rand = new Random();

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.AskTime, context.CancellationToken);

                    for (int i = 0; i < _sensors.Count; i++)
                    {
                        Data.SensorInfo sensor = _sensors[i];

                        WeatherMock.Generate(ref sensor);

                        var itemResponse = new EventResponse()
                        {
                            EventId = rand.Next(),
                            SensorInfo = new SensorInfo() { Id = sensor.Id, Name = sensor.Name, SensorType = (SensorTypeEnum)(int)sensor.SensorType },
                            Temperature = sensor.Temperature,
                            Humidity = sensor.Humidity,
                            Co2 = sensor.CO2,
                            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                        };

                        await responseStream.WriteAsync(itemResponse, context.CancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation cancelled");
            }
        }
    }
}