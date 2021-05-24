using BaseRunner;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.Log.Runner.AspDI
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
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();


                var insertTelemetryRequest = new InsertTelemetryItem
                {
                    ConnectionName = "SampleConnectionName",
                    DateTimeUTC = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(100),
                    Request = "SampleSignalRequest",
                    TelemetryResponseState = TelemetryResponseState.Successful,
                    TelemetryType = TelemetryType.Application,
                    CorrelationId = "SampleCorrelationId"
                };

                telemetryWriterService.Insert(insertTelemetryRequest);

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

            serviceCollection.AddLoggerService();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddTelemetryWriterService();
            
            serviceCollection.AddSinksTelemetryLogServiceService();

            return serviceCollection;
        }
    }
}
