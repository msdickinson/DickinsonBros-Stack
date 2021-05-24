using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Configurators;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Factories;
using DickinsonBros.Infrastructure.AzureTables.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
         class SampleTestAzureTableServiceOptionsType : AzureTableServiceOptionsType { }
         class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };
    

        [TestMethod]
        public void AddAzureTablesService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddAzureTablesService<SampleTestAzureTableServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IAzureTableService<SampleTestAzureTableServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(AzureTableService<SampleTestAzureTableServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<AzureTableServiceOptions<SampleTestAzureTableServiceOptionsType>>) &&
                               serviceDefinition.ImplementationType == typeof(AzureTableServiceOptionsConfigurator<SampleTestAzureTableServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICloudStorageAccountFactory) &&
                               serviceDefinition.ImplementationType == typeof(CloudStorageAccountFactory) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICloudTableClientFactory) &&
                               serviceDefinition.ImplementationType == typeof(CloudTableClientFactory) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
