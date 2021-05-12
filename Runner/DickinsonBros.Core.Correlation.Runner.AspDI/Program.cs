using BaseRunner;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Correlation.Runner.AspDI
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
                var correlationService = provider.GetRequiredService<ICorrelationService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                Console.WriteLine($"This shows that if a async method changes the value, it will stay changed in that context");
                Console.WriteLine($"but will not change the outer context value");
                Console.WriteLine($"");

                correlationService.CorrelationId = "100";

                Console.WriteLine($"CorrelationId Before Run: {correlationService.CorrelationId}");

                await ModifyCorrelationIdAsync("200", correlationService).ConfigureAwait(false);

                Console.WriteLine($"CorrelationId After Run: {correlationService.CorrelationId}");

                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task ModifyCorrelationIdAsync(string updateValue, ICorrelationService correlationService)
        {
            Console.WriteLine($"CorrelationId Before Modify: {correlationService.CorrelationId}");
            correlationService.CorrelationId = updateValue;
            Console.WriteLine($"CorrelationId After Modify: {correlationService.CorrelationId}");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            serviceCollection.AddCorrelationService();

            return serviceCollection;
        }
    }
}
