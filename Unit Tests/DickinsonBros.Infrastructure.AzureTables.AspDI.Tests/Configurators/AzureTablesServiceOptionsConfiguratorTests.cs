using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Configurators;
using DickinsonBros.Infrastructure.AzureTables.Models;
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
    public class AzureTablesServiceOptionsConfiguratorTests : BaseTest
    {
        public class SampleTestAzureTableServiceOptionsType : AzureTableServiceOptionsType { }
        public class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var azureTableServiceOptions = new AzureTableServiceOptions<SampleTestAzureTableServiceOptionsType>
            {
                ConnectionString = "SampleConnectionString"
            };

            var azureTableServiceOptionsDecrypted = new AzureTableServiceOptions<SampleTestAzureTableServiceOptionsType>
            {
                ConnectionString = "SampleConnectionStringDecrypted"
            };


            var configurationRoot = BuildConfigurationRoot(azureTableServiceOptions);

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
                            azureTableServiceOptions.ConnectionString
                        )
                    )
                    .Returns
                    (
                        azureTableServiceOptionsDecrypted.ConnectionString
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<AzureTableServiceOptions<SampleTestAzureTableServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(azureTableServiceOptionsDecrypted.ConnectionString, options.ConnectionString);

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
            serviceCollection.AddSingleton<IConfigureOptions<AzureTableServiceOptions<SampleTestAzureTableServiceOptionsType>>, AzureTableServiceOptionsConfigurator<SampleTestAzureTableServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>>();
            serviceCollection.AddSingleton(Mock.Of<ICertificateEncryptionService<SampleTestCertificateEncryptionServiceOptionsType>>());

            return serviceCollection;
        }

        #endregion
    }
}
