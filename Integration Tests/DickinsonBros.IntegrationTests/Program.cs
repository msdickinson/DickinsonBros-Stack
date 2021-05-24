using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
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
using DickinsonBros.IntegrationTests.Config;
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
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.AzureTables.Extensions;
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.Log.Extensions;
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
        internal const string AZURE_TABLE_NAME_TELEMETRY = "DickinsonBrosIntegrationTestsTelemetry";

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
                var tests               = integrationTestService.FetchTestsByGroup("Sinks");

                var testSummary         = await integrationTestService.RunTests(tests).ConfigureAwait(false);
                var testlog             = integrationTestService.GenerateLog(testSummary, false);
                Console.WriteLine(testlog);

                await Task.CompletedTask.ConfigureAwait(false);
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
            //all azure calls -or at least delete remove logs when not want them here to clear properly

            //Azure Tables
            var azureTableService = provider.GetRequiredService<IAzureTableService<RunnerAzureTableServiceOptionsType>>();

            var tableQuery = new TableQuery<SampleEntity>();
            var items = await azureTableService.QueryAsync(AZURE_TABLE_NAME, tableQuery).ConfigureAwait(false);
            await azureTableService.DeleteBulkAsync(items, AZURE_TABLE_NAME).ConfigureAwait(false);

            var tableQueryTelemetry = new TableQuery<SampleEntity>();
            var itemsTelemetry = await azureTableService.QueryAsync(AZURE_TABLE_NAME_TELEMETRY, tableQueryTelemetry).ConfigureAwait(false);
            await azureTableService.DeleteBulkAsync(itemsTelemetry, AZURE_TABLE_NAME_TELEMETRY).ConfigureAwait(false);
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

            //--Infrastructure
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Middleware

            //--Application

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

            //--Middleware

            //--Application

            //--Sinks
            serviceCollection.AddSinksTelemetryLogIntegrationTests();
            serviceCollection.AddSinksTelemetryAzureTablesIntegrationTests();
        }
    }
}
