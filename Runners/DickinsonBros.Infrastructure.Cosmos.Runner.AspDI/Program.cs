using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Config;
using DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Extensions;
using DickinsonBros.Infrastructure.Cosmos.Runner.AspDI.Config;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Runner.AspDI
{
    class Program
    {
        private const string KEY = "SampleCosmosRunner";

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
                var cosmosService = provider.GetRequiredService<ICosmosService<RunnerCosmosServiceOptionsType>>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();

                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var sampleModelValue = GenerateNewSampleModel();
                var sampleModelValue2 = GenerateNewSampleModel();
                var sampleModelValues = new List<SampleModel>();
                for (var i = 0; i < 3; i++)
                {
                    sampleModelValues.Add(GenerateNewSampleModel());
                }


                //Insert
                var insertResult = await cosmosService.InsertAsync(sampleModelValue).ConfigureAwait(false);

                //Upsert
                insertResult.Resource.SampleData = "NewDATA";
                var resultTwo = await cosmosService.UpsertAsync(insertResult.Resource).ConfigureAwait(false);

                //Delete
                var deleteResult = await cosmosService.DeleteAsync<SampleModel>(insertResult.Resource.Id, insertResult.Resource.Key).ConfigureAwait(false);

                //Insert Bulk 
                var bulkInsertResult = await cosmosService.InsertBulkAsync(sampleModelValues).ConfigureAwait(false);

                //Query
                var queryResult = await cosmosService.QueryAsync<SampleModel>
                                   (
                                       new QueryDefinition($"SELECT TOP 10 * FROM c where c.key='{KEY}'"),
                                       new QueryRequestOptions
                                       {
                                           PartitionKey = new PartitionKey(KEY),
                                           MaxItemCount = 10
                                       }
                                   ).ConfigureAwait(false);

                //Upsert Bulk
                queryResult.ToList().ForEach(sampleModel => sampleModel.SampleData += " Edited");
                var upsertBulkResult = await cosmosService.UpsertBulkAsync(queryResult).ConfigureAwait(false);

                //Delete Bulk 
                var deleteBulkResult = await cosmosService.DeleteBulkAsync<SampleModel>(queryResult).ConfigureAwait(false);

                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private SampleModel GenerateNewSampleModel()
        {
            var guid = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            var key = KEY;

            return new SampleModel
            {
                Id = guid,
                Key = key,
                SampleData = value
            };
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

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
            serviceCollection.AddCosmosService<RunnerCosmosServiceOptionsType, Configuration>();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();


            return serviceCollection;
        }
    }
}
