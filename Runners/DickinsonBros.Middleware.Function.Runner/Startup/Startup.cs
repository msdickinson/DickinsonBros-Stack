using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using Dickinsonbros.Middleware.Function.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Middleware.Function.Runner.Config;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;

[assembly: WebJobsStartup(typeof(Dickinsonbros.Middleware.Function.Runner.Startup.Startup))]
namespace Dickinsonbros.Middleware.Function.Runner.Startup
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        const string _siteRootPath = @"\home\site\wwwroot\";
        const string FUNCTION_ENVIRONMENT_NAME = "FUNCTION_ENVIRONMENT_NAME";
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = EnrichConfiguration(builder.Services);
            ConfigureServices(builder.Services, configuration);
        }
        private IConfiguration EnrichConfiguration(IServiceCollection serviceCollection)
        {
            var existingConfiguration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddConfiguration(existingConfiguration);
            var configTransform = $"appsettings.{System.Environment.GetEnvironmentVariable(FUNCTION_ENVIRONMENT_NAME)}.json";
            var isCICD = !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), configTransform));
            var functionConfigurationRootPath = isCICD ? _siteRootPath : Directory.GetCurrentDirectory();
            var config =
                configurationBuilder
                .SetBasePath(functionConfigurationRootPath)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile(configTransform, false)
                .Build();
            serviceCollection.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));

            return config;
        }
        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

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
            services.AddJWTEncryptionService<DickinsonBros.Middleware.Function.Runner.Config.Runner, Configuration>();

            //--Infrastructure
            services.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            services.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            services.AddSinksTelemetryLogServiceService();

            //#Local Packages
            services.AddMiddlwareFunctionService<DickinsonBros.Middleware.Function.Runner.Config.Runner, Configuration>(configuration);
        }

    }
}

