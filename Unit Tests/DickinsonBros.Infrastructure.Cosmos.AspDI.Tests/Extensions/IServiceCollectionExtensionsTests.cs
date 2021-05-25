using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Configurators;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        class SampleTestCosmosServiceOptionsType : CosmosServiceOptionsType { }
        class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public void AddAzureTablesService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddCosmosService<SampleTestCosmosServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICosmosService<SampleTestCosmosServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(CosmosService<SampleTestCosmosServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<CosmosServiceOptions<SampleTestCosmosServiceOptionsType>>) &&
                               serviceDefinition.ImplementationType == typeof(CosmosServiceOptionsConfigurator<SampleTestCosmosServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICosmosFactory) &&
                               serviceDefinition.ImplementationType == typeof(CosmosFactory) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
