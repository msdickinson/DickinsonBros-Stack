using BaseRunner;
using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Redactor.Runner.AspDI
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
                var redactorService = provider.GetRequiredService<IRedactorService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run application
                var sampleContract = new SampleContract
                {
                    Password = "SamplePassword",
                    Username = "SampleUsername"
                };

                var input =
    @"{
""Password"": ""password""
}";
                Console.WriteLine($"Raw Json: \r\n {input}");
                Console.WriteLine($"Redacted Json: \r\n { redactorService.Redact(input)}");

                Console.WriteLine($"Sample Model \r\n { System.Text.Json.JsonSerializer.Serialize(sampleContract)}");
                Console.WriteLine($"Redacted Model: \r\n { redactorService.Redact(sampleContract)}");

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

            serviceCollection.AddRedactorService();

            return serviceCollection;
        }
    }

    public class SampleContract
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
