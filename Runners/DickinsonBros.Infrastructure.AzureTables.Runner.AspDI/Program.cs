using BaseRunner;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Config;
using DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Models;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Runner.AspDI
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
                var azureTableService = provider.GetRequiredService<IAzureTableService<RunnerAzureTableServiceOptionsType>>();
                var telemetryWriterService = provider.GetRequiredService<ITelemetryWriterService>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var sampleEntity = GenerateNewSampleEntity();
                var sampleTableName = "SampleTable";

                var insertAsyncResult = await azureTableService.InsertAsync(sampleEntity, sampleTableName).ConfigureAwait(false);
                Console.WriteLine("InsertAsync");
                Console.WriteLine(JsonSerializer.Serialize(insertAsyncResult, new JsonSerializerOptions() { WriteIndented = true }));
                Console.WriteLine("");

                var fetchAsyncResult = await azureTableService.FetchAsync<SampleEntity>(sampleEntity.PartitionKey, sampleEntity.RowKey, sampleTableName).ConfigureAwait(false);
                Console.WriteLine("FetchAsync");
                Console.WriteLine(JsonSerializer.Serialize(fetchAsyncResult, new JsonSerializerOptions() { WriteIndented = true }));
                Console.WriteLine("");

                hostApplicationLifetime.StopApplication();

                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private SampleEntity GenerateNewSampleEntity()
        {
            var sampleEntity = new SampleEntity();
            sampleEntity.URL = "https://www.google.com/";
            sampleEntity.Pass = true;
            sampleEntity.PartitionKey = System.DateTime.UtcNow.ToShortDateString();
            sampleEntity.RowKey = Guid.NewGuid().ToString();
            sampleEntity.PartitionKey = "PartitionKey";
            sampleEntity.Timestamp = DateTime.UtcNow;
            return sampleEntity;
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            serviceCollection.AddDateTimeService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddCertificateEncryptionService<Configuration>();
            serviceCollection.AddSinksTelemetryLogServiceService();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            return serviceCollection;
        }
    }
}
