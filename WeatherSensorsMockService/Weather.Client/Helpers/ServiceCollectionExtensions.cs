using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Weather.Client.Options;

namespace Weather.Client.Helpers
{
    /// <summary>
    /// Extensions for service setup
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add GRPC clients
        /// </summary>
        /// <param name="services"> Services </param>
        /// <param name="configuration"> Configuration </param>
        public static void AddLocalGrpcClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLocalGrpcClient<Weather.Emulator.Generator.GeneratorClient, WeatherGrpcConfig>(configuration);
        }

        /// <summary>
        /// Add GRPC client
        /// </summary>
        /// <typeparam name="TClient"> Client type </typeparam>
        /// <typeparam name="TOptions"> Client options </typeparam>
        /// <param name="services"> Services </param>
        /// <param name="configuration"> Configuration </param>
        private static void AddLocalGrpcClient<TClient, TOptions>(
                  this IServiceCollection services,
                  IConfiguration configuration)
                  where TClient : ClientBase
                  where TOptions : BaseGrpcConfig, new()
        {
            services.AddGrpcClient<TClient>((
                _,
                config) =>
            {
                var options = new TOptions();
                var section = configuration.GetSection(options.GetSectionPath());
                section.Bind(options);
                var url = options.Url;
                config.Address = new Uri(url!);
                config.ChannelOptionsActions.Add(channelOptions =>
                {
                    channelOptions.MaxSendMessageSize = options.SendMessageSize;
                    channelOptions.MaxReceiveMessageSize = options.ReceiveMessageSize;
                });
            });
        }
    }
}
