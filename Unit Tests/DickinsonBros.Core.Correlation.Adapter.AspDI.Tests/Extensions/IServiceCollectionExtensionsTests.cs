using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Core.Correlation.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddCorrelationService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddCorrelationService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICorrelationService) &&
                                           serviceDefinition.ImplementationType == typeof(CorrelationService) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
