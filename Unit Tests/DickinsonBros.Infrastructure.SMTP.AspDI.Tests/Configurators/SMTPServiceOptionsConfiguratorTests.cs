using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.AspDI.Configurators;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SMTP.AspDI.Tests.Configurators
{

    [TestClass]
    public class SMTPServiceOptionsConfiguratorTests : BaseTest
    {
        public class SampleTestSMTPServiceOptionsType : SMTPServiceOptionsType { }
        public class SampleTestCertificateEncryptionServiceOptionsType : CertificateEncryptionServiceOptionsType { };

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var smtpServiceOptionsType = new SMTPServiceOptions<SampleTestSMTPServiceOptionsType>
            {
                EmailTimeout = new System.TimeSpan(0,0,30),
                Host = "SampleHost",
                IdealEmailLoad = 25,
                InactivityTimeout = new System.TimeSpan(0, 1, 0),
                MaxConnections = 5,
                Password = "SamplePassword",
                Port = 500,
                UserName = "SampleUserName"
            };

            var smtpServiceOptionsDecrypted = new SMTPServiceOptions<SampleTestSMTPServiceOptionsType>
            {
                Password = "SamplePasswordDecrypted"
            };

            var configurationRoot = BuildConfigurationRoot(smtpServiceOptionsType);

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
                            smtpServiceOptionsType.Password
                        )
                    )
                    .Returns
                    (
                        smtpServiceOptionsDecrypted.Password
                    );

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<SampleTestSMTPServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(smtpServiceOptionsDecrypted.Password, options.Password);
                    Assert.AreEqual(smtpServiceOptionsType.Host, options.Host);
                    Assert.AreEqual(smtpServiceOptionsType.IdealEmailLoad, options.IdealEmailLoad);
                    Assert.AreEqual(smtpServiceOptionsType.MaxConnections, options.MaxConnections);
                    Assert.AreEqual(smtpServiceOptionsType.Port, options.Port);
                    Assert.AreEqual(smtpServiceOptionsType.UserName, options.UserName);

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
            serviceCollection.AddSingleton<IConfigureOptions<SMTPServiceOptions<SampleTestSMTPServiceOptionsType>>, SMTPServiceOptionsConfigurator<SampleTestSMTPServiceOptionsType, SampleTestCertificateEncryptionServiceOptionsType>>();
            serviceCollection.AddSingleton(Mock.Of<ICertificateEncryptionService<SampleTestCertificateEncryptionServiceOptionsType>>());

            return serviceCollection;
        }

        #endregion
    }
}
