using DickinsonBros.Encryption.AES.Abstractions.Models;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.AES.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.AES.Adapter.AspDI.Tests.Configurators
{
    [TestClass]
    public class AESEncryptionServiceOptionsConfiguratorTests : BaseTest
    {
        public class TestClass : AESEncryptionServiceOptionsType
        {

        }

        [TestMethod]
        public async Task Configure_Runs_OptionsReturned()
        {
            var certificateEncryptionServiceOptionsOfObject = new AESEncryptionServiceOptions<TestClass>
            {
                InitializationVector = "SampleInitializationVector",
                Key = "SampleKey"
            };
            var configurationRoot = BuildConfigurationRoot(certificateEncryptionServiceOptionsOfObject);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<AESEncryptionServiceOptions<TestClass>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(certificateEncryptionServiceOptionsOfObject.InitializationVector, options.InitializationVector);
                    Assert.AreEqual(certificateEncryptionServiceOptionsOfObject.Key, options.Key);

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
            serviceCollection.AddSingleton<IConfigureOptions<AESEncryptionServiceOptions<TestClass>>, AESEncryptionServiceOptionsConfigurator<TestClass>>();

            return serviceCollection;
        }

        #endregion
    }
}
