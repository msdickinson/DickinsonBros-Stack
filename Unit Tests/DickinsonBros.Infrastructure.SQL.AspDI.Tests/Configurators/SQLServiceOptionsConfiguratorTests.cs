using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.AspDI.Configurators;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Tests.Configurators
{

    [TestClass]
    public class SQLServiceOptionsConfiguratorTests : BaseTest
    {
        public class SampleTestSQLServiceOptionsType : SQLServiceOptionsType { }
        public class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var sqlServiceOptionsType = new SQLServiceOptions<SampleTestSQLServiceOptionsType>
            {
                ConnectionString = "SampleConnectionString",
                ConnectionName = "SampleConnectionName"
            };

            var sqlServiceOptionsDecrypted = new SQLServiceOptions<SampleTestSQLServiceOptionsType>
            {
                ConnectionString = "SampleConnectionStringDecrypted"
            };


            var configurationRoot = BuildConfigurationRoot(sqlServiceOptionsType);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--IConfigurationEncryptionService
                    var configurationEncryptionServiceMock = serviceProvider.GetMock<ICertificateEncryptionService<SampleTestCertificateEncryptionServiceOptionsType>>();

                    configurationEncryptionServiceMock
                    .Setup
                    (
                        configurationEncryptionService => configurationEncryptionService.Decrypt
                        (
                            sqlServiceOptionsType.ConnectionString
                        )
                    )
                    .Returns
                    (
                        sqlServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<SQLServiceOptions<SampleTestSQLServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(sqlServiceOptionsDecrypted.ConnectionString, options.ConnectionString);
                    Assert.AreEqual(sqlServiceOptionsType.ConnectionName, options.ConnectionName);


                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<SQLServiceOptions<SampleTestSQLServiceOptionsType>>, SQLServiceOptionsConfigurator<SampleTestSQLServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>>();
            serviceCollection.AddSingleton(Mock.Of<ICertificateEncryptionService<SampleTestCertificateEncryptionServiceOptionsType>>());

            return serviceCollection;
        }

        #endregion
    }
}
