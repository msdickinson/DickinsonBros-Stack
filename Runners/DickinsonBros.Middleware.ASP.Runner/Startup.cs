using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Middleware.ASP.Runner.Config;
using DickinsonBros.MiddlewareService.ASP;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DickinsonBros.Middleware.ASP.Runner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();

            //--Core
            services.AddGuidService();
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddStopwatchFactory();
            services.AddLoggerService();
            services.AddCorrelationService();
            services.AddRedactorService();
            services.AddTelemetryWriterService();

            //--Encryption
            services.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            services.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            services.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            services.AddSinksTelemetryLogServiceService();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Adds Middleware
            app.UseMiddleware<MiddlewareASPService>();

            //Comes with ASP.net Template
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
