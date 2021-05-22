using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        public class SampleAzureTableServiceOptionsType : AzureTableServiceOptionsType{};

        [TestMethod]
        public void AddSinksTelemetryLogService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddSinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetryWriterService) &&
                                           serviceDefinition.ImplementationType == typeof(SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
