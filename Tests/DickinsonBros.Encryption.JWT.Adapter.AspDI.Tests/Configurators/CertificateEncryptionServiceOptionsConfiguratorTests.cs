using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.JWT.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.JWT.Adapter.AspDI.Tests.Configurators
{
    [TestClass]
    public class CertificateEncryptionServiceOptionsConfiguratorTests : BaseTest
    {
        public class TestJWTEncryptionServiceOptionsType : JWTEncryptionServiceOptionsType
        {
        }

        public class TestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType
        {
        }

        [TestMethod]
        public async Task Configure_Runs_OptionsReturned()
        {
            var jwtServiceOptions = new JWTEncryptionServiceOptions<TestJWTEncryptionServiceOptionsType, TestCertificateEncryptionServiceOptionsType>
            {
                AccessExpiresAfterMinutes = 1,
                Audience = "SampleAudience",
                RefershExpiresAfterMinutes = 2,
                Issuer = "SampleIssuer",
            };
            var configurationRoot = BuildConfigurationRoot(jwtServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<JWTEncryptionServiceOptions<TestJWTEncryptionServiceOptionsType, TestCertificateEncryptionServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(jwtServiceOptions.AccessExpiresAfterMinutes, options.AccessExpiresAfterMinutes);
                    Assert.AreEqual(jwtServiceOptions.Audience, options.Audience);
                    Assert.AreEqual(jwtServiceOptions.RefershExpiresAfterMinutes, options.RefershExpiresAfterMinutes);
                    Assert.AreEqual(jwtServiceOptions.Issuer, options.Issuer);

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
            serviceCollection.AddSingleton<IConfigureOptions<JWTEncryptionServiceOptions<TestJWTEncryptionServiceOptionsType, TestCertificateEncryptionServiceOptionsType>>, JWTServiceOptionsConfigurator<TestJWTEncryptionServiceOptionsType, TestCertificateEncryptionServiceOptionsType>>();

            return serviceCollection;
        }

        #endregion
    }
}
