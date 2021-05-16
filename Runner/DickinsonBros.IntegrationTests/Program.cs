using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.IntegrationTests.Tests.Core.Correlation.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.DateTime.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Guid.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Logger.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Redactor.Extensions;
using DickinsonBros.IntegrationTests.Tests.Core.Stopwatch.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.AES.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.Certificate.Extensions;
using DickinsonBros.IntegrationTests.Tests.Encryption.JWT.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Extensions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Models;
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.AzureTables.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Adapter.AspDI.Extensions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
{
    class Program
    {
        internal const string AZURE_TABLE_NAME = "DickinsonBrosIntegrationTests";

        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            var serviceCollection = ConfigureServices();
            using var provider = serviceCollection.BuildServiceProvider();
            provider.ConfigureAwait(true);
            var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();

            try
            {
                //var tests             = integrationTestService.FetchTestsByTestName("InsertBulkAndUpsertBulkAsync_Runs_IsSuccessful");
                var tests               = integrationTestService.FetchTestsByName("AzureTables");

                var testSummary         = await integrationTestService.RunTests(tests).ConfigureAwait(false);
                var testlog             = integrationTestService.GenerateLog(testSummary, false);
                Console.WriteLine(testlog);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            finally
            {
                await AzureTablesCleanUpAsync(provider).ConfigureAwait(false);
            }
        }

        private async Task AzureTablesCleanUpAsync(ServiceProvider provider)
        {
            //Azure Tables
            var azureTableService = provider.GetRequiredService<IAzureTableService<RunnerAzureTableServiceOptionsType>>();

            var tableQuery = new TableQuery<SampleEntity>();

            var items = await azureTableService.QueryAsync(AZURE_TABLE_NAME, tableQuery).ConfigureAwait(false);
            var result = await azureTableService.DeleteBulkAsync(items, AZURE_TABLE_NAME).ConfigureAwait(false);
        }

        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            //Add Services
            serviceCollection.AddGuidService();
            serviceCollection.AddDateTimeService();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddIntegrationTestService();
            serviceCollection.AddAESEncryptionService<RunnerAESEncryptionServiceOptionsType>();
            serviceCollection.AddCertificateEncryptionService<Configuration>();
            serviceCollection.AddJWTEncryptionService<RunnerJWTEncryptionServiceOptionsType, Configuration>();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();
            
            //serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();

            serviceCollection.AddDateTimeService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddCertificateEncryptionService<Configuration>();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //Add Integreation Tests
            serviceCollection.AddGuidIntegrationTests();
            serviceCollection.AddDateTimeIntegrationTests();
            serviceCollection.AddCorrelationIntegrationTests();
            serviceCollection.AddLoggerIntegrationTests();
            serviceCollection.AddStopwatchIntegrationTests();
            serviceCollection.AddRedactorIntegrationTests();
            serviceCollection.AddAESIntegrationTests();
            serviceCollection.AddCertificateIntegrationTests();
            serviceCollection.AddJWTIntegrationTests();
            serviceCollection.AddAzureTablesIntegrationTests();
            serviceCollection.AddSinksTelemetryAzureTablesIntegrationTests();

            return serviceCollection;
        }
    }
}
