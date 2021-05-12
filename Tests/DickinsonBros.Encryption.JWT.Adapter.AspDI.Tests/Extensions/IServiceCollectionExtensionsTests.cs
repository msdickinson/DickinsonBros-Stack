using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.JWT.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Encryption.JWT.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        public class TestJWTServiceOptions : JWTEncryptionServiceOptionsType
        {
        }
        public class TestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType
        {
        }

        [TestMethod]
        public void AddSQLService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddJWTEncryptionService<TestJWTServiceOptions, TestCertificateEncryptionServiceOptionsType>();

            // Assert

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IJWTEncryptionService<TestJWTServiceOptions, TestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.ImplementationType == typeof(JWTEncryptionService<TestJWTServiceOptions, TestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<JWTEncryptionServiceOptions<TestJWTServiceOptions>>) &&
                               serviceDefinition.ImplementationType == typeof(JWTServiceOptionsConfigurator<TestJWTServiceOptions>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
