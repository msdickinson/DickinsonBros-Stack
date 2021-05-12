using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Encryption.Certificate.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        public class Sample : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public void AddCertificateEncryptionService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddCertificateEncryptionService<Sample>();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICertificateEncryptionService<Sample>) &&
                                           serviceDefinition.ImplementationType == typeof(CertificateEncryptionService<Sample>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<CertificateEncryptionServiceOptions<Sample>>) &&
                               serviceDefinition.ImplementationType == typeof(CertificateEncryptionServiceOptionsConfigurator<Sample>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }

    }
}
