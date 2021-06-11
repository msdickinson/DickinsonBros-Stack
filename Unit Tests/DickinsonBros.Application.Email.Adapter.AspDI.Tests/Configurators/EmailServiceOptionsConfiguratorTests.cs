using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Application.Email.Adapter.AspDI.Configurators;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Application.Email.Adapter.AspDI.Tests.Configurators
{
    [TestClass]
    public class EmailServiceOptionsConfiguratorTests : BaseTest
    {
        public class SampleTestSMTPServiceOptionsType : SMTPServiceOptionsType { }

        [TestMethod]
        public async Task Configure_Runs_ExpectedOptions()
        {
            var emailServiceOptionsType = new EmailServiceOptions<SampleTestSMTPServiceOptionsType>
            {
                SaveDirectory = "SampleFilePath",
                SaveToFile = true,
                SendSMTP = true
            };

            var configurationRoot = BuildConfigurationRoot(emailServiceOptionsType);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<SampleTestSMTPServiceOptionsType>>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(emailServiceOptionsType.SaveDirectory, options.SaveDirectory);
                    Assert.AreEqual(emailServiceOptionsType.SaveToFile, options.SaveToFile);
                    Assert.AreEqual(emailServiceOptionsType.SendSMTP, options.SendSMTP);

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
            serviceCollection.AddSingleton<IConfigureOptions<EmailServiceOptions<SampleTestSMTPServiceOptionsType>>, EmailServiceOptionsConfigurator<SampleTestSMTPServiceOptionsType>>();

            return serviceCollection;
        }

        #endregion
    }
}
