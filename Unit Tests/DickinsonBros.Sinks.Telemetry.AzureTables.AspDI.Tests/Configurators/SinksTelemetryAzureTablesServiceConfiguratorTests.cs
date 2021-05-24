using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Configurators;
using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Tests.Configurators
{
    [TestClass]
    public class SinksTelemetryAzureTablesServiceConfiguratorTests : BaseTest
    {
        public class TestClass : AzureTableServiceOptionsType { }

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var sinksTelemetryAzureTablesServiceOptions = new SinksTelemetryAzureTablesServiceOptions
            {
                PartitionKey = "SampleStoreLocation",
                TableName = "SampleThumbPrint",
                UploadInterval = new TimeSpan(0, 5, 0)
            };

            var configurationRoot = BuildConfigurationRoot(sinksTelemetryAzureTablesServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<SinksTelemetryAzureTablesServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(sinksTelemetryAzureTablesServiceOptions.PartitionKey, options.PartitionKey);
                    Assert.AreEqual(sinksTelemetryAzureTablesServiceOptions.TableName, options.TableName);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<SinksTelemetryAzureTablesServiceOptions>, SinksTelemetryAzureTablesServiceConfigurator>();

            return serviceCollection;
        }

        #endregion
    }
}
