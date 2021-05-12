using DickinsonBros.Encryption.AES.Abstractions;
using DickinsonBros.Encryption.AES.Abstractions.Models;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.AES.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace DickinsonBros.Encryption.AES.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        public class Sample : AESEncryptionServiceOptionsType { };

        [TestMethod]
        public void AddAESEncryptionService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAESEncryptionService<Sample>();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IAESEncryptionService<Sample>) &&
                                           serviceDefinition.ImplementationType == typeof(AESEncryptionService<Sample>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<AESEncryptionServiceOptions<Sample>>) &&
                   serviceDefinition.ImplementationType == typeof(AESEncryptionServiceOptionsConfigurator<Sample>) &&
                   serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
