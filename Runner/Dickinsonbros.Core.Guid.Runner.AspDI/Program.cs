using BaseRunner;
using Dickinsonbros.Core.Guid.Abstractions;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.Guid.Runner.AspDI
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
                var guidService = provider.GetRequiredService<IGuidService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                Console.WriteLine(guidService.NewGuid());

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

            serviceCollection.AddGuidService();

            return serviceCollection;
        }
    }
}
