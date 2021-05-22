using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Core.DateTime.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddDateTimeService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddDateTimeService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IDateTimeService) &&
                                                       serviceDefinition.ImplementationType == typeof(DateTimeService) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
