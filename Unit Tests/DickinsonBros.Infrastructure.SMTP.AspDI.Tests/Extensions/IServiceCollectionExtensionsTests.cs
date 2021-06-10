using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.AspDI.Configurators;
using DickinsonBros.Infrastructure.SMTP.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.SMTP.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        class SampleTestSMTPServiceOptionsType : SMTPServiceOptionsType { }
        class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public void AddAzureTablesService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddSMTPService<SampleTestSMTPServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ISMTPService<SampleTestSMTPServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(SMTPService<SampleTestSMTPServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<SMTPServiceOptions<SampleTestSMTPServiceOptionsType>>) &&
                               serviceDefinition.ImplementationType == typeof(SMTPServiceOptionsConfigurator<SampleTestSMTPServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
