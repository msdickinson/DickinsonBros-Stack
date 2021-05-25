using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Configurators;
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
    public class CosmosServiceOptionsConfiguratorTests : BaseTest
    {
        public class SampleTestCosmosServiceOptionsType : CosmosServiceOptionsType { }
        public class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var cosmosServiceOptionsType = new CosmosServiceOptions<SampleTestCosmosServiceOptionsType>
            {
                ConnectionString = "SampleConnectionString",
                ContainerId = "SampleContainerId",
                DatabaseId = "SampleDatabaseId"
            };

            var cosmosServiceOptionsDecrypted = new CosmosServiceOptions<SampleTestCosmosServiceOptionsType>
            {
                ConnectionString = "SampleConnectionStringDecrypted"
            };


            var configurationRoot = BuildConfigurationRoot(cosmosServiceOptionsType);

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
                            cosmosServiceOptionsType.ConnectionString
                        )
                    )
                    .Returns
                    (
                        cosmosServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<CosmosServiceOptions<SampleTestCosmosServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(cosmosServiceOptionsDecrypted.ConnectionString, options.ConnectionString);
                    Assert.AreEqual(cosmosServiceOptionsType.ContainerId, options.ContainerId);
                    Assert.AreEqual(cosmosServiceOptionsType.DatabaseId, options.DatabaseId);


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
            serviceCollection.AddSingleton<IConfigureOptions<CosmosServiceOptions<SampleTestCosmosServiceOptionsType>>, CosmosServiceOptionsConfigurator<SampleTestCosmosServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>>();
            serviceCollection.AddSingleton(Mock.Of<ICertificateEncryptionService<SampleTestCertificateEncryptionServiceOptionsType>>());

            return serviceCollection;
        }

        #endregion
    }
}
