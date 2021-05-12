using BaseRunner;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.Stopwatch.Runner.AspDI
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
                var stopwatchService = provider.GetRequiredService<IStopwatchService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                Console.WriteLine("Start Timer And Wait 1 Seconds");
                stopwatchService.Start();
                System.Threading.Thread.Sleep(1000);
                stopwatchService.Stop();
                Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

                Console.WriteLine("Start Continue 2 Seconds");
                stopwatchService.Start();
                System.Threading.Thread.Sleep(1000);
                stopwatchService.Stop();
                Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

                Console.WriteLine("Start Timer Reset");
                stopwatchService.Reset();
                Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

                Console.WriteLine("Start Timer And Wait 1 Seconds");
                stopwatchService.Start();
                System.Threading.Thread.Sleep(1000);
                stopwatchService.Stop();
                Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

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

            serviceCollection.AddStopwatchService();

            return serviceCollection;
        }
    }
}
