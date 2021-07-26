using BaseRunner;
using Dickinsonbros.Core.Guid.Abstractions;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Config;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Models.Models;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy.Models;
using DickinsonBros.Infrastructure.SMTP.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Rest.Runner.AspDI
{
    public class ItemWithCallBackMethod
    {
        public string Input;
        public Task CallBackMethod;
    }

    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        public async Task ExecuteAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            finally
            {
            }
        }

        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                using var provider = serviceCollection.BuildServiceProvider();
                var restService = provider.GetRequiredService<IRestService>();
                var correlationService = provider.GetRequiredService<ICorrelationService>();
                var guidService = provider.GetRequiredService<IGuidService>();
                var jsonPlaceHolderProxyService = provider.GetRequiredService<IJsonPlaceHolderProxyService>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();

                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                await Task.WhenAll
                (
                    Request(correlationService, guidService, restService),
                    RequestOfT(correlationService, guidService, restService),
                    RequestUsingProxy(jsonPlaceHolderProxyService, correlationService, guidService)
                ).ConfigureAwait(false);

                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task Request(ICorrelationService correlationService, IGuidService guidService, IRestService restService)
        {
            correlationService.CorrelationId = guidService.NewGuid().ToString();

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("todos/1", UriKind.Relative)
            };

            var retrys = 3;
            var timeoutInSeconds = 30;
            var httpResponseMessage = await restService.ExecuteAsync("https://jsonplaceholder.typicode.com/todos/", httpClient, httpRequestMessage, retrys, timeoutInSeconds).ConfigureAwait(false);
            Console.WriteLine("Content: " + await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        private async Task RequestOfT(ICorrelationService correlationService, IGuidService guidService, IRestService restService)
        {
            correlationService.CorrelationId = guidService.NewGuid().ToString();

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("todos/1", UriKind.Relative)
            };

            var retrys = 3;
            var timeoutInSeconds = 30;

            var restResponse = await restService.ExecuteAsync<Todo>("https://jsonplaceholder.typicode.com/todos/", httpClient, httpRequestMessage, retrys, timeoutInSeconds).ConfigureAwait(false);

            Console.WriteLine("Content: " + await restResponse.HttpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        private async Task RequestUsingProxy(IJsonPlaceHolderProxyService jsonPlaceHolderProxyService, ICorrelationService correlationService, IGuidService guidService)
        {
            correlationService.CorrelationId = guidService.NewGuid().ToString();
            var restResponse = await jsonPlaceHolderProxyService.GetTodosAsync(new GetTodosRequest
            {
                Items = 2
            }).ConfigureAwait(false);


            Console.WriteLine("Content: " + await restResponse.HttpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            //--Misc

            //--Core
            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();

            //--Encryption
            serviceCollection.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            serviceCollection.AddRestService();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();

            //Local Services
            serviceCollection.AddHttpClient<IJsonPlaceHolderProxyService, JsonPlaceHolderProxyService>(client =>
            {
                client.BaseAddress = new Uri(configruation[$"{nameof(JsonPlaceHolderProxyOptions)}:{nameof(JsonPlaceHolderProxyOptions.BaseURL)}"]);
            });
            serviceCollection.Configure<JsonPlaceHolderProxyOptions>(configruation.GetSection(nameof(JsonPlaceHolderProxyOptions)));


            return serviceCollection;
        }
    }
}
