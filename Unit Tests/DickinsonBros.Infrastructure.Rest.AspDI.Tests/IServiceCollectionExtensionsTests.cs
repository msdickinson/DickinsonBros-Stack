using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.Infrastructure.Rest.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.Rest.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddRestService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddRestService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IRestService) &&
                                                       serviceDefinition.ImplementationType == typeof(RestService) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
