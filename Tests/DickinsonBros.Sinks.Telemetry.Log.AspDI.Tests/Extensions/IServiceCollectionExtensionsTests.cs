using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Sinks.Telemetry.Log.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddSinksTelemetryLogService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddSinksTelemetryLogServiceService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetryServiceWriter) &&
                                           serviceDefinition.ImplementationType == typeof(SinksTelemetryLogService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
