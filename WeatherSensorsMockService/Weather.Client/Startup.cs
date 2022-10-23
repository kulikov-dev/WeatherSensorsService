using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weather.Client.Helpers;
using Weather.Client.HostedServices;
using Weather.Client.Models;
using Weather.Client.Options;
using Weather.Data;

namespace Weather.Client
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceConfig>(_configuration.GetSection("ServiceConfig"));
            services.AddLocalGrpcClients(_configuration);

            services.AddHostedService<WeatherHostedService>();
            services.AddSingleton<ISensorsStorage, SensorsStorage>();
            services.AddSingleton<ISubscriptionStorage, SubscriptionStorage>();

            services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"> IApplicationBuilder </param>
        /// <param name="env"> IWebHostEnvironment </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}