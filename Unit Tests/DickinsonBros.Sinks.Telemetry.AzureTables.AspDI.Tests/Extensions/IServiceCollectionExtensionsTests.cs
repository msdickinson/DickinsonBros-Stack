using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Configurators;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        class SampleAzureTableServiceOptionsType : AzureTableServiceOptionsType{}

        [TestMethod]
        public void AddSinksTelemetryAzureTablesService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddSinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));


            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<SinksTelemetryAzureTablesServiceOptions>) &&
                               serviceDefinition.ImplementationType == typeof(SinksTelemetryAzureTablesServiceConfigurator) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
