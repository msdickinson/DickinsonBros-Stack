using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Runner.AspDI.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Runner.AspDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                using var provider = serviceCollection.BuildServiceProvider();
                var telemetryWriterService = provider.GetRequiredService<ITelemetryWriterService>();
                var correlationService = provider.GetRequiredService<ICorrelationService>();

                correlationService.CorrelationId = "SampleCorrelationId";

                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var insertTelemetryRequest = new InsertTelemetryItem
                {
                    ConnectionName = "SampleConnectionName",
                    DateTimeUTC = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(100),
                    Request = "SampleSignalRequest",
                    TelemetryResponseState = TelemetryResponseState.Successful,
                    TelemetryType = TelemetryType.Application,
                    CorrelationId = correlationService.CorrelationId
                };

                telemetryWriterService.Insert(insertTelemetryRequest);
                await Task.Delay(10000).ConfigureAwait(false);

                var insertTelemetryRequest2 = new InsertTelemetryItem
                {
                    ConnectionName = "SampleConnectionName",
                    DateTimeUTC = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(100),
                    Request = "SampleSignalRequest2",
                    TelemetryResponseState = TelemetryResponseState.Successful,
                    TelemetryType = TelemetryType.Application,
                    CorrelationId = correlationService.CorrelationId
                };
                telemetryWriterService.Insert(insertTelemetryRequest2);
                await sinksTelemetryAzureTablesService.FlushAsync().ConfigureAwait(false);

                hostApplicationLifetime.StopApplication();

                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            //--Core
            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();

            //--Encryption
            serviceCollection.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();

            return serviceCollection;
        }
    }
}
