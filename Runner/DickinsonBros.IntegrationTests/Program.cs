using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.DateTime.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Guid.Extensions;
using DickinsonBros.Test.Integration;
using DickinsonBros.Test.Integration.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
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
                var runner = provider.GetRequiredService<IRunner>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run Tests
                var result = await runner.RunAsync().ConfigureAwait(false);
                Console.WriteLine(result);


                provider.ConfigureAwait(true);
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

            //Local
            serviceCollection.TryAddSingleton<IRunner, Runner>();

            //Add Services
            serviceCollection.AddGuidService();
            serviceCollection.AddDateTimeService();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddIntegrationTestService();

            //Add Integreation Tests
            serviceCollection.AddGuidIntegrationTests();
            serviceCollection.AddDateTimeIntegrationTests();

            return serviceCollection;
        }
    }
}
