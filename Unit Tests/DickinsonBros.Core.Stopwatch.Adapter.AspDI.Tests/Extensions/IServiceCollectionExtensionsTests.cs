using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Core.Stopwatch.Adapter.AspDI.Tests.Extensions

{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddStopwatchService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddStopwatchService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IStopwatchService) &&
                                                       serviceDefinition.ImplementationType == typeof(StopwatchService) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }

        [TestMethod]
        public void AddStopwatchFactory_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddStopwatchFactory();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IStopwatchFactory) &&
                                                       serviceDefinition.ImplementationType == typeof(StopwatchFactory) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
