using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Application.Email.Adapter.AspDI.Configurators;
using DickinsonBros.Application.Email.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Application.Email.Adapter.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        class SampleTestSMTPServiceOptionsType : SMTPServiceOptionsType { }

        [TestMethod]
        public void AddEmailService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddEmailService<SampleTestSMTPServiceOptionsType>();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IEmailService<SampleTestSMTPServiceOptionsType>) &&
                                           serviceDefinition.ImplementationType == typeof(EmailService<SampleTestSMTPServiceOptionsType>) &&
                                           serviceDefinition.Lifetime == ServiceLifetime.Singleton));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<EmailServiceOptions<SampleTestSMTPServiceOptionsType>>) &&
                               serviceDefinition.ImplementationType == typeof(EmailServiceOptionsConfigurator<SampleTestSMTPServiceOptionsType>) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));

        }
    }
}
