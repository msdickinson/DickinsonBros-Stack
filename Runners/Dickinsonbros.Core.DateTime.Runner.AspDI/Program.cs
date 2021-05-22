using BaseRunner;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.DateTime.Runner.AspDI
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
                var dateTimeService = provider.GetRequiredService<IDateTimeService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var datetime = dateTimeService.GetDateTimeUTC();
                Console.WriteLine(datetime);

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

            return serviceCollection;
        }
    }
}
