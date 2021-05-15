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
using DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.AzureTables.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
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
                provider.ConfigureAwait(true);
                var integrationTestService = provider.GetRequiredService<IIntegrationTestService>();

                //Run Tests
                var tests               = integrationTestService.FetchTestsByTestName("InsertAndUpsert_Runs_ValuesMatch");
                //var tests               = integrationTestService.FetchTestsByName("AzureTables");

                var testSummary         = await integrationTestService.RunTests(tests).ConfigureAwait(false);
                var testlog             = integrationTestService.GenerateLog(testSummary, false);
                Console.WriteLine(testlog);
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
