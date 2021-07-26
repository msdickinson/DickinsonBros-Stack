using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Application.Email.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Extensions;
using DickinsonBros.Infrastructure.DNS.AspDI.Extensions;
using DickinsonBros.Infrastructure.File.AspDI.Extensions;
using DickinsonBros.Infrastructure.SMTP.AspDI.Extensions;
using DickinsonBros.Infrastructure.SQL.AspDI.Extensions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.IntegrationTests.Tests.Application.Email.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Correlation.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.DateTime.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Guid.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Logger.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Redactor.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Stopwatch.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Telemetry.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.AES.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.Certificate.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.JWT.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Models;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Models;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.DNS.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.File.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.Rest.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.SMTP.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.SQL.Extensions;
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.AzureTables.Extensions;
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.Log.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Adapter.AspDI.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
{
    //TODO: Change Sample Emails to keep subject unquie
    //
    class Program
    {
        internal const string AZURE_TABLE_NAME = "DickinsonBrosIntegrationTests";
        internal const string AZURE_TABLE_NAME_TELEMETRY = "DickinsonBrosIntegrationTestsTelemetry";
        internal const string COSMOS_KEY = "DickinsonBrosIntegrationTests";
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            var serviceCollection = ConfigureServices();
            using var provider = serviceCollection.BuildServiceProvider();
            provider.ConfigureAwait(true);


            //These have to pulled here to ensure there constuctor is called. 
            var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();
            var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();

            var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();
            var testlog = (string)null;
            try
            {
                var tests               = integrationTestService.FetchTestsByName("Rest");
                var testSummary         = await integrationTestService.RunTests(tests).ConfigureAwait(false);
                testlog                 = integrationTestService.GenerateLog(testSummary, false);

                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {   
                //Flush
                await FlushAzureTablesCleanUpAsync(provider).ConfigureAwait(false);

                //Clear
                await CosmosCleanUpAsync(provider).ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);
                await AzureTablesCleanUpAsync(provider).ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine(testlog);
            }
        }
        private async Task FlushAzureTablesCleanUpAsync(ServiceProvider provider)
        {
            var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();
            await sinksTelemetryAzureTablesService.FlushAsync().ConfigureAwait(false);
        }
        private async Task AzureTablesCleanUpAsync(ServiceProvider provider)
        {
            var azureTableService = provider.GetRequiredService<IAzureTableService<RunnerAzureTableServiceOptionsType>>();
            var tableQuery = new TableQuery<SampleEntity>();
            var items = await azureTableService.QueryAsync(AZURE_TABLE_NAME, tableQuery, false).ConfigureAwait(false);
            await azureTableService.DeleteBulkAsync(items, AZURE_TABLE_NAME, false).ConfigureAwait(false);

            var tableQueryTelemetry = new TableQuery<SampleEntity>();
            var itemsTelemetry = await azureTableService.QueryAsync(AZURE_TABLE_NAME_TELEMETRY, tableQueryTelemetry, false).ConfigureAwait(false);
            await azureTableService.DeleteBulkAsync(itemsTelemetry, AZURE_TABLE_NAME_TELEMETRY, false).ConfigureAwait(false);
        }
        private async Task CosmosCleanUpAsync(ServiceProvider provider)
        {
            //Azure Tables
            var cosmosService = provider.GetRequiredService<ICosmosService<RunnerCosmosServiceOptionsType>>();

            var queryResult = await cosmosService.QueryAsync<SampleModel>
                                   (
                                       new QueryDefinition($"SELECT TOP 1000 * FROM c where c.key='{COSMOS_KEY}'"),
                                       new QueryRequestOptions
                                       {
                                           PartitionKey = new PartitionKey(COSMOS_KEY),
                                           MaxItemCount = 100
                                       }
                                   ).ConfigureAwait(false);

            //Bulk Delete
            await cosmosService.DeleteBulkAsync<SampleModel>(queryResult).ConfigureAwait(false);

        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            ConfigureStack(serviceCollection);
            ConfigureIntegreationTests(serviceCollection);

            return serviceCollection;
        }
        private void ConfigureStack(IServiceCollection serviceCollection)
        {
            //--Misc
            serviceCollection.AddMemoryCache();
            serviceCollection.TryAddSingleton<IFileSystem, FileSystem>();

            //--Core
            serviceCollection.AddGuidService();
            serviceCollection.AddDateTimeService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();

            //--Test
            serviceCollection.AddIntegrationTestService();

            //--Encryption
            serviceCollection.AddAESEncryptionService<RunnerAESEncryptionServiceOptionsType>();
            serviceCollection.AddCertificateEncryptionService<Configuration>();
            serviceCollection.AddJWTEncryptionService<RunnerJWTEncryptionServiceOptionsType, Configuration>();
            serviceCollection.AddFileService();

            //--Infrastructure
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();
            serviceCollection.AddCosmosService<RunnerCosmosServiceOptionsType, Configuration>();
            serviceCollection.AddSQLService<RunnerSQLServiceOptionsType, Configuration>();
            serviceCollection.AddSMTPService<RunnerSMTPServiceOptionsType, Configuration>();
            serviceCollection.AddRestService();
            serviceCollection.AddDNSService();

            //--Middleware

            //--Application
            serviceCollection.AddEmailService<RunnerSMTPServiceOptionsType>();

            //--Sinks
            serviceCollection.AddSinksTelemetryLogServiceService();
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
        }
        private void ConfigureIntegreationTests(IServiceCollection serviceCollection)
        {
            //--Core
            serviceCollection.AddGuidIntegrationTests();
            serviceCollection.AddDateTimeIntegrationTests();
            serviceCollection.AddCorrelationIntegrationTests();
            serviceCollection.AddLoggerIntegrationTests();
            serviceCollection.AddStopwatchIntegrationTests();
            serviceCollection.AddRedactorIntegrationTests();
            serviceCollection.AddTelemetryIntegrationTests();

            //--Test

            //--Encryption
            serviceCollection.AddAESIntegrationTests();
            serviceCollection.AddCertificateIntegrationTests();
            serviceCollection.AddJWTIntegrationTests();

            //--Infrastructure
            serviceCollection.AddAzureTablesIntegrationTests();
            serviceCollection.AddCosmosIntegrationTests();
            serviceCollection.AddSQLIntegrationTests();
            serviceCollection.AddFileIntegrationTests();
            serviceCollection.AddSMTPIntegrationTests();
            serviceCollection.AddDNSIntegrationTests();
            serviceCollection.AddRestIntegrationTests();


            //--Middleware

            //--Application
            serviceCollection.AddEmailIntegrationTests();

            //--Sinks
            serviceCollection.AddSinksTelemetryLogIntegrationTests();
            serviceCollection.AddSinksTelemetryAzureTablesIntegrationTests();
        }
    }
}
