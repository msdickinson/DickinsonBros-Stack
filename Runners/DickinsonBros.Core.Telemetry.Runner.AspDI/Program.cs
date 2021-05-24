using BaseRunner;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Telemetry.Runner.AspDI
{
    class Program
    {
        private readonly List<TelemetryItem> _telemetryItems = new List<TelemetryItem>();

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
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                Console.WriteLine($"Wire Up New Telemetry Event");
                telemetryWriterService.NewTelemetryEvent += TelemetryServiceWriter_NewTelemetryEvent;

                Console.WriteLine($"Telemetry Items: {_telemetryItems.Count()}");

                var telemetryItem = new InsertTelemetryItem()
                {
                    DateTimeUTC = DateTime.UtcNow,
                    ConnectionName = "SampleConnectionName",
                    Duration = TimeSpan.FromSeconds(1),
                    Request = "SampleSignalRequest",
                    CorrelationId = "SampleCorrelationId",
                    TelemetryResponseState = TelemetryResponseState.Successful,
                    TelemetryType = TelemetryType.Application
                };

                Console.WriteLine($"Insert TelemetryItem");
                telemetryWriterService.Insert(telemetryItem);

                var count = _telemetryItems.Count();

                Console.WriteLine($"Telemetry Items: {_telemetryItems.Count()}");

                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TelemetryServiceWriter_NewTelemetryEvent(TelemetryItem telemetryItem)
        {
            _telemetryItems.Add(telemetryItem);
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            serviceCollection.AddTelemetryWriterService();
            return serviceCollection;
        }
    }
}
