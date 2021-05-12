using BaseRunner;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Logger.Runner.AspDI
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

                //Start application
                using var provider = serviceCollection.BuildServiceProvider();
                var loggingService = provider.GetRequiredService<ILoggerService<Program>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run application
                var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };

                var message = "Generic Log Message";
                var exception = new Exception("Error");

                loggingService.LogDebugRedacted(message);
                loggingService.LogDebugRedacted(message, data);

                loggingService.LogInformationRedacted(message);
                loggingService.LogInformationRedacted(message, data);

                loggingService.LogWarningRedacted(message);
                loggingService.LogWarningRedacted(message, data);

                loggingService.LogErrorRedacted(message, exception);
                loggingService.LogErrorRedacted(message, exception, data);


                //Stop application
                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("End...");
                Console.ReadKey();
            }
        }



        private IServiceCollection ConfigureServices()
        {
            var configuration = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configuration);

            serviceCollection.AddLoggerService();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();

            return serviceCollection;
        }
    }
}
