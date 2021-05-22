using Dickinsonbros.Core.Guid.Abstractions;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dickinsonbros.Core.Guid.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddPropelAPI_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddGuidService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IGuidService) &&
                                                       serviceDefinition.ImplementationType == typeof(GuidService) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
