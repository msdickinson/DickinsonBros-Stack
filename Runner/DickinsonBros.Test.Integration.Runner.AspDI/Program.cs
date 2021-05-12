using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Adapter.AspDI.Extensions;
using DickinsonBros.Test.Integration.Runner.AspDI.ExampleTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
namespace DickinsonBros.Test.Integration.Runner.AspDI
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
                var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Setup Tests
                var exampleTests = new ExampleTests();
                var tests = integrationTestService.SetupTests(exampleTests);

                //Run Tests
                var testSummary = await integrationTestService.RunTests(tests).ConfigureAwait(false);

                //Process Test Summary
                var trxReport = integrationTestService.GenerateTRXReport(testSummary);
                var log = integrationTestService.GenerateLog(testSummary, true);

                //Console Summary
                Console.WriteLine("Log:");
                Console.WriteLine(log);
                Console.WriteLine();

                Console.WriteLine("TRX Report:");
                Console.WriteLine(trxReport);
                Console.WriteLine();

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

            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddIntegrationTestService();

            serviceCollection.AddStopwatchFactory();

            return serviceCollection;
        }
    }
}
