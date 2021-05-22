using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Core.Logger.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddLoggerService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddLoggerService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ILoggerService<>) &&
                                           serviceDefinition.ImplementationType == typeof(LoggerService<>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
