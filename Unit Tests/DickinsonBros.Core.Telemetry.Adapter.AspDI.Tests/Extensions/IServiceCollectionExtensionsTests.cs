using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Configurators;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DickinsonBros.Core.Telemetry.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddTelemetryWriterService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddTelemetryWriterService();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ITelemetryWriterService) &&
                                           serviceDefinition.ImplementationType == typeof(TelemetryWriterService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<TelemetryWriterServiceOptions>) &&
                               serviceDefinition.ImplementationType == typeof(TelemetryWriterServiceOptionsConfigurator) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }

    }
}
