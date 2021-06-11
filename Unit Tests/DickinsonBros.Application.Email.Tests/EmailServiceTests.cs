using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Infrastructure.File.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Application.Email.Tests
{
    [TestClass]
    public class EmailServiceTests : BaseTest
    {

        #region IsVaildEmailFormat

        [TestMethod]
        public void IsValidEmailFormat_Runs_LogInformationRedactedCalled()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@email.com";

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    var result = uutConcrete.IsValidEmailFormat(email);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogInformationRedacted
                        (
                            $"{nameof(EmailService<Test>)}<{typeof(Test).Name}>.{nameof(EmailService<Test>.IsValidEmailFormat)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );

                    var emailObserved = (string)propertiesObserved.First()["email"];
                    var isVaildObserved = (bool)propertiesObserved.First()["isVaild"];
                    var regexMatchTimeoutObserved = (bool)propertiesObserved.First()["regexMatchTimeout"];
                  
                    Assert.AreEqual(email, emailObserved);
                    Assert.IsTrue(isVaildObserved);
                    Assert.IsFalse(regexMatchTimeoutObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void IsValidEmailFormat_EmptyOrWhiteSpace_ReturnsFalse()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var email = "";

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    var result = uutConcrete.IsValidEmailFormat(email);

                    //Assert
                    Assert.IsFalse(result);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void IsValidEmailFormat_Vaild_ReturnsTrue()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@email.com";

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    var result = uutConcrete.IsValidEmailFormat(email);

                    //Assert
                    Assert.IsTrue(result);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region SendAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendAsync_NullInput_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var mimeMessage = (MimeMessage)null;

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    await uutConcrete.SendAsync(mimeMessage).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendAsync_EmailServiceOptionsSaveToFileTrue_FileServiceUpsertFileAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var mimeMessage = new MimeMessage();

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- IFileService
                    var fileServiceMock = CreateFileServiceMock(serviceProvider);

                    //-- IGuidService
                    var guid = new Guid();
                    var guidServiceMock = CreateGuidServiceMock(serviceProvider, guid);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;
                    var expectedPath = emailServiceOptions.SaveDirectory + "\\" + guid.ToString() + ".eml";

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    await uutConcrete.SendAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    fileServiceMock.Verify
                    (
                        fileService => fileService.UpsertFileAsync
                        (
                            expectedPath,
                            It.IsAny<byte[]>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendAsync_EmailServiceOptionsSendSMTPTrue_SmtpServiceSendEmailAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var mimeMessage = new MimeMessage();

                    //-- ISMTPServiceMock
                    var sendEmailDescriptor = new SendEmailDescriptor
                    {
                        Exception = null,
                        SendEmailResult = SendEmailResult.Successful
                    };

                    var smptServiceMock = CreateSMTPServiceMock(serviceProvider, sendEmailDescriptor);

                    //-- IFileService
                    var fileServiceMock = CreateFileServiceMock(serviceProvider);

                    //-- IGuidService
                    var guid = new Guid();
                    var guidServiceMock = CreateGuidServiceMock(serviceProvider, guid);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var emailServiceOptions = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions<Test>>>().Value;
                    var expectedPath = emailServiceOptions.SaveDirectory + "\\" + guid.ToString() + ".eml";

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<IEmailService<Test>>();
                    var uutConcrete = (EmailService<Test>)uut;

                    //Act
                    await uutConcrete.SendAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    smptServiceMock.Verify
                    (
                        smptService => smptService.SendEmailAsync
                        (
                            It.IsAny<MimeMessage>()
                        ),
                        Times.Once()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }


        #endregion

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //--Core
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<EmailService<Test>>>());
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());

            //--Encryption

            //--Infrastructure
            serviceCollection.AddSingleton(Mock.Of<IFileService>());
            serviceCollection.AddSingleton(Mock.Of<ISMTPService<Test>>());

            //--Application
            serviceCollection.AddSingleton<IEmailService<Test>, EmailService<Test>>();

            //--Options
            serviceCollection.AddOptions();
            var emailServiceOptions = new EmailServiceOptions<Test>
            {
                SaveDirectory = "SampleFilePath",
                SaveToFile = true,
                SendSMTP = true,

            };
            var options = Options.Create(emailServiceOptions);
            serviceCollection.AddSingleton<IOptions<EmailServiceOptions<Test>>>(options);

            var configurationRoot = BuildConfigurationRoot(emailServiceOptions);
            serviceCollection.AddSingleton<IConfiguration>(configurationRoot);


            return serviceCollection;
        }

        private Mock<IFileService> CreateFileServiceMock(IServiceProvider serviceProvider)
        {
            var fileServiceMock = serviceProvider.GetMock<IFileService>();
            fileServiceMock.Setup
            (
                fileService => fileService.UpsertFileAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()
                )
           );
            return fileServiceMock;
        }


        private Mock<ISMTPService<Test>> CreateSMTPServiceMock(IServiceProvider serviceProvider, SendEmailDescriptor sendEmailDescriptor)
        {
            var smtpServiceMock = serviceProvider.GetMock<ISMTPService<Test>>();
            smtpServiceMock.Setup
            (
                smtpService => smtpService.SendEmailAsync
                (
                    It.IsAny<MimeMessage>()
                )
           )
           .ReturnsAsync
           (
                sendEmailDescriptor
           );

            return smtpServiceMock;
        }


        private Mock<IGuidService> CreateGuidServiceMock(IServiceProvider serviceProvider, Guid guid)
        {
            var guidServiceMock = serviceProvider.GetMock<IGuidService>();
            guidServiceMock.Setup
            (
                guidService => guidService.NewGuid()
            )
            .Returns
            (
                guid
            );

            return guidServiceMock;
        }

        private (Mock<ILoggerService<EmailService<Test>>>, List<Dictionary<string, object>>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<EmailService<Test>>>();
            var propertiesObserved = new List<Dictionary<string, object>>();
            loggerServiceMock
            .Setup
            (
                loggerService => loggerService.LogInformationRedacted
                (
                    It.IsAny<string>(),
                    It.IsAny<LogGroup>(),
                    It.IsAny<IDictionary<string, object>>()
                )
            ).Callback((string message, LogGroup LogGroup, IDictionary<string, object> properties) =>
            {
                var props = new Dictionary<string, object>();
                foreach (var prop in properties)
                {
                    props.Add(prop.Key, prop.Value);
                }

                propertiesObserved.Add(props);
            });

            loggerServiceMock
            .Setup
            (
                loggerService => loggerService.LogErrorRedacted
                (
                    It.IsAny<string>(),
                    It.IsAny<LogGroup>(),
                    It.IsAny<Exception>(),
                    It.IsAny<IDictionary<string, object>>()
                )
            ).Callback((string message, LogGroup LogGroup, Exception exception, IDictionary<string, object> properties) =>
            {
                var props = new Dictionary<string, object>();

                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        props.Add(prop.Key, prop.Value);
                    }
                }
                propertiesObserved.Add(props);
            });

            return (loggerServiceMock, propertiesObserved);
        }
        #endregion

        public class Test : SMTPServiceOptionsType
        {
        }
    }
}
