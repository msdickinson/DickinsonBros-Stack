using BaseRunner;
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
                var xService = provider.GetRequiredService<SinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

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
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();

            return serviceCollection;
        }
    }
}
