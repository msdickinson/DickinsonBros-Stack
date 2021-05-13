using BaseRunner;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
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
                var sinksTelemetryLogService = provider.GetRequiredService<ITelemetryServiceWriter>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var insertTelemetryRequest = new InsertTelemetryRequest
                {
                    ConnectionName = "SampleConnectionName",
                    DateTimeUTC = DateTime.UtcNow,
                    Duration = TimeSpan.FromSeconds(100),
                    SignalRequest = "SampleSignalRequest",
                    SignalResponse = "SampleSignalResponse",
                    TelemetryResponseState = TelemetryResponseState.Successful,
                    TelemetryType = TelemetryType.Application
                };

                await sinksTelemetryLogService.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);

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
            serviceCollection.AddSinksTelemetryLogServiceService();

            return serviceCollection;
        }
    }
}
