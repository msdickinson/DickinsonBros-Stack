using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.AspDI.Configurators;
using DickinsonBros.Infrastructure.SQL.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        class SampleTestSQLServiceOptionsType : SQLServiceOptionsType { }
        class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public void AddAzureTablesService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddSQLService<SampleTestSQLServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ISQLService<SampleTestSQLServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(SQLService<SampleTestSQLServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<SQLServiceOptions<SampleTestSQLServiceOptionsType>>) &&
                               serviceDefinition.ImplementationType == typeof(SQLServiceOptionsConfigurator<SampleTestSQLServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IDataTableService) &&
                               serviceDefinition.ImplementationType == typeof(DataTableService) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
