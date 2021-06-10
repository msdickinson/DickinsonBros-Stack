using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Test.Unit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SMTP.Tests
{
    [TestClass]
    public class SMTPServiceTests : BaseTest
    {
        #region SendEmailAsync

        [TestMethod]
        public async Task SendEmailAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Runs_StopwatchStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchServiceMock => stopwatchServiceMock.Start(),
                        Times.Exactly(3)
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Runs_GetDateTimeUTCCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage(); 
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_NullInput_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await uutConcrete.SendEmailAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Runs_StopwatchStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchServiceMock => stopwatchServiceMock.Stop(),
                        Times.Exactly(1)
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_TaskIsExceptional_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider, true);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogErrorRedacted
                        (
                            $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.SendEmailAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );

                    var hostObserved = (string)propertiesObserved.First()["Host"];
                    var subjectObserved = (string)propertiesObserved.First()["Subject"];
                    var toObserved = (string)propertiesObserved.First()["To"];
                    var fromObserved = (string)propertiesObserved.First()["From"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(smtpServiceOptions.Value.Host, hostObserved);
                    Assert.AreEqual(mimeMessage.Subject, subjectObserved);
                    Assert.AreEqual(mimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address).First(), toObserved);
                    Assert.AreEqual(mimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address).First(), fromObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_TaskIsNotExceptional_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogInformationRedacted
                        (
                            $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.SendEmailAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );

                    var hostObserved = (string)propertiesObserved.First()["Host"];
                    var subjectObserved = (string)propertiesObserved.First()["Subject"];
                    var toObserved = (string)propertiesObserved.First()["To"];
                    var fromObserved = (string)propertiesObserved.First()["From"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(smtpServiceOptions.Value.Host, hostObserved);
                    Assert.AreEqual(mimeMessage.Subject, subjectObserved);
                    Assert.AreEqual(mimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address).First(), toObserved);
                    Assert.AreEqual(mimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address).First(), fromObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_TaskIsNotExceptional_ReturnSendEmailDescriptorFromTask()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsNull(observed.Exception);
                    Assert.AreEqual(SendEmailResult.Successful, observed.SendEmailResult);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Exceptional_LogErrorRedacted()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();
                    smtpServiceOptions.Value.InactivityTimeout = new TimeSpan(0);
                    smtpServiceOptions.Value.IdealEmailLoad = 0;

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogErrorRedacted
                        (
                            $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.SendEmailAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );

                    var hostObserved = (string)propertiesObserved.First()["Host"];
                    var subjectObserved = (string)propertiesObserved.First()["Subject"];
                    var toObserved = (string)propertiesObserved.First()["To"];
                    var fromObserved = (string)propertiesObserved.First()["From"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(smtpServiceOptions.Value.Host, hostObserved);
                    Assert.AreEqual(mimeMessage.Subject, subjectObserved);
                    Assert.AreEqual(mimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address).First(), toObserved);
                    Assert.AreEqual(mimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address).First(), fromObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Exceptional_ReturnNewSendEmailDescriptor()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();
                    smtpServiceOptions.Value.InactivityTimeout = new TimeSpan(0);
                    smtpServiceOptions.Value.IdealEmailLoad = 0;

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.IsNotNull(observed.Exception);
                    Assert.AreEqual(SendEmailResult.Failed, observed.SendEmailResult);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SendEmailAsync_Runs_telemetryWriterServiceInsertCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("SampleFromEmailName", "sampleFrom@email.com"));
                    mimeMessage.To.Add(new MailboxAddress("SampleToEmailName", "sampleTo@email.com"));

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.SendEmailAsync(mimeMessage).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(smtpServiceOptions.Value.Host, insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.SMTP, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region BalanceSmtpClients

        [TestMethod]
        public async Task BalanceSmtpClients_NoExistingClients_AddsClients()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    for(int i = 0; i < 10; i++)
                    {
                        uutConcrete.emailRequests.Enqueue
                        (
                            new EmailRequest
                            {
                                MimeMessage = mimeMessage,
                                CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                            }
                        );
                    }

                    //Act
                    uutConcrete.BalanceSmtpClients();

                    //Assert
                    Assert.AreEqual(2, uutConcrete.smtpClientHandlers.Count());

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task BalanceSmtpClients_FullClients_DoesNotAddClients()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    uutConcrete.smtpClientHandlers.TryAdd(new Task(async () => await Task.CompletedTask.ConfigureAwait(false)), "");
                    uutConcrete.smtpClientHandlers.TryAdd(new Task(async () => await Task.CompletedTask.ConfigureAwait(false)), "");

                    for (int i = 0; i < 10; i++)
                    {
                        uutConcrete.emailRequests.Enqueue
                        (
                            new EmailRequest
                            {
                                MimeMessage = mimeMessage,
                                CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                            }
                        );
                    }

                    //Act
                    uutConcrete.BalanceSmtpClients();

                    //Assert
                    Assert.AreEqual(2, uutConcrete.smtpClientHandlers.Count());
                    foreach (var handle in uutConcrete.smtpClientHandlers)
                    {
                        handle.Key.Start();
                    }

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }
        #endregion

        #region HandledAwaited

        [TestMethod]
        public async Task HandledAwaited_TaskIsExceptional_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethodThrow();
                    uutConcrete.emailRequests.Enqueue
                    (
                        new EmailRequest
                        {
                            MimeMessage = mimeMessage,
                            CallBack = taskA
                        }
                    );

                    //Act
                    uutConcrete.HandledAwaited(taskA);
                    await Task.Delay(100);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogErrorRedacted
                        (
                            $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.HandledAwaited)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task HandledAwaited_Runs_RemoveItemFromPool()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = Task.Delay(100).ContinueWith(async (task) => { await Task.CompletedTask.ConfigureAwait(false); throw new Exception(); });
                    uutConcrete.smtpClientHandlers.TryAdd(taskA, "");
                    
                    uutConcrete.emailRequests.Enqueue
                    (
                        new EmailRequest
                        {
                            MimeMessage = mimeMessage,
                            CallBack = taskA
                        }
                    );

                    //Act
                    uutConcrete.HandledAwaited(taskA);
                    await Task.Delay(200);

                    //Assert
                    Assert.IsFalse(uutConcrete.smtpClientHandlers.ContainsKey(taskA));

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task HandledAwaited_ExitOnFlushIsFalse_AddtionalPoolItemAdded()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethod();
                    uutConcrete.smtpClientHandlers.TryAdd(taskA, "");
                    uutConcrete.emailRequests.Enqueue
                    (
                        new EmailRequest
                        {
                            MimeMessage = mimeMessage,
                            CallBack = taskA
                        }
                    );

                    //Act
                    uutConcrete.HandledAwaited(taskA);
                    await Task.Delay(200);

                    //Assert
                    Assert.IsFalse(uutConcrete.smtpClientHandlers.Any());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task HandledAwaited_ExitOnFlushIsTrue_PoolIsEmpty()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;
                    uutConcrete.ExitOnFlush = true;
                    var taskA = Task.Delay(50).ContinueWith(async (task) => { await Task.CompletedTask.ConfigureAwait(false); });
                    uutConcrete.smtpClientHandlers.TryAdd(taskA, "");
                    uutConcrete.emailRequests.Enqueue
                    (
                        new EmailRequest
                        {
                            MimeMessage = mimeMessage,
                            CallBack = taskA
                        }
                    );

                    //Act
                    uutConcrete.HandledAwaited(taskA);
                    await Task.Delay(100);

                    //Assert
                    Assert.IsFalse(uutConcrete.smtpClientHandlers.Any());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task HandledAwaited_finallyIsExceptional_DoesNotThrow()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();


                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();
                    smtpServiceOptions.Value.IdealEmailLoad = 0;

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethodThrow();
                    uutConcrete.emailRequests.Enqueue
                    (
                        new EmailRequest
                        {
                            MimeMessage = mimeMessage,
                            CallBack = taskA
                        }
                    );

                    //Act
                    uutConcrete.HandledAwaited(taskA);
                    await Task.Delay(100);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }
        //HandledAwaited_AwaitTask_TaskRuns
        //HandledAwaited_AwaitTask_TaskRuns
        //HandledAwaited_AwaitTaskAndItThrows_LogErrorRedactedCalled
        #endregion

        #region SmtpClientProcessQueueItemsAsync

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_Runs_SMTPClientFactoryCreateCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpFactoryMock
                    .Verify
                    (
                        smtpFactory => smtpFactory.Create(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_Runs_SMTPClientConnectAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();
                    smtpServiceOptions.Value.InactivityTimeout = new TimeSpan(0);


                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpClientMock
                    .Verify
                    (
                        smtpClientMock => smtpClientMock.ConnectAsync
                        (
                            smtpServiceOptions.Value.Host,
                            smtpServiceOptions.Value.Port,
                            SecureSocketOptions.StartTls,
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_Runs_SMTPClientAuthenticateAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //-- IOptions<SMTPServiceOptions<Test>>
                    var smtpServiceOptions = serviceProvider.GetRequiredService<IOptions<SMTPServiceOptions<Test>>>();
                    smtpServiceOptions.Value.InactivityTimeout = new TimeSpan(0);


                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpClientMock
                    .Verify
                    (
                        smtpClientMock => smtpClientMock.AuthenticateAsync
                        (
                            smtpServiceOptions.Value.UserName,
                            smtpServiceOptions.Value.Password,
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_Runs_StopWatchStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_EmailRequestsExist_SMTPClientSendAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethod();
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = taskA
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpClientMock
                    .Verify
                    (
                        smtpClient => smtpClient.SendAsync
                        (
                            request.MimeMessage,
                            It.IsAny<CancellationToken>(),
                            It.IsAny<ITransferProgress>()
                        ),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_EmailRequestsExist_EmailRequestCallBackCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethod();
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = taskA
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    Assert.IsTrue(request.CallBack.Status != TaskStatus.WaitingForActivation);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_EmailRequestsExist_StopWatchResetCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock.Verify
                    (
                        stopwatchService => stopwatchService.Reset(),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_EmailRequestsExist_StopWatchStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethod();
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = taskA
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock.Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_InactivityTimeout_SMTPClientDisconnectAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskA = SampleMethod();
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpClientMock.Verify
                    (
                        smtpClient => smtpClient.DisconnectAsync
                        (
                            true,
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Exactly(1)
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_ExitOnFlushIsTrue_SMTPClientDisconnectAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    uutConcrete.ExitOnFlush = true;

                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    smtpClientMock.Verify
                    (
                        smtpClient => smtpClient.DisconnectAsync
                        (
                            true,
                            It.IsAny<CancellationToken>()
                        ),
                        Times.AtLeastOnce()
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_IsExceptionalAndEmailRequestIsNull_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider, false, true);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    uutConcrete.ExitOnFlush = true;
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerServiceMock => loggerServiceMock.LogErrorRedacted
                        (
                            $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.SmtpClientProcessQueueItemsAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<Dictionary<string, object>>()
                        ),
                        Times.Exactly(1)
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_IsExceptionalAndEmailRequestIsNotNull_SendEmailDescriptorContainsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider, true);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    uutConcrete.ExitOnFlush = true;
                    var taskA = SampleMethod();
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = taskA
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(SendEmailResult.Failed, request.SendEmailDescriptor.SendEmailResult);
                    Assert.IsNotNull(request.SendEmailDescriptor.Exception);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SmtpClientProcessQueueItemsAsync_IsExceptionalAndCallBackStatusIsCreated_EmailRequestCallBackCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider, true);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    uutConcrete.ExitOnFlush = true;
                    var request = new EmailRequest
                    {
                        MimeMessage = mimeMessage,
                        CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
                    };
                    uutConcrete.emailRequests.Enqueue
                    (
                        request
                    );

                    //Act
                    await uutConcrete.SmtpClientProcessQueueItemsAsync().ConfigureAwait(false);

                    //Assert
                    Assert.AreNotEqual(TaskStatus.Created, request.CallBack.Status);

                    //loggerServiceMock
                    //.Verify
                    //(
                    //    loggerServiceMock => loggerServiceMock.LogErrorRedacted
                    //    (
                    //        $"{nameof(SMTPService<Test>)}<{typeof(Test).Name}>.{nameof(SMTPService<Test>.SmtpClientProcessQueueItemsAsync)}",
                    //        LogGroup.Infrastructure,
                    //        It.IsAny<Exception>(),
                    //        It.IsAny<Dictionary<string, object>>()
                    //    ),
                    //    Times.Exactly(1)
                    //);

                    //var elapsedObserved = (TimeSpan)propertiesObserved.First()["Elapsed"];
                    //Assert.IsNotNull(elapsedObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region FlushAsync

        [TestMethod]
        public async Task FlushAsync_Runs_AllSMTPClientHandlersTasksAreComplete()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var mimeMessage = new MimeMessage();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //-- ISMTPClient
                    var smtpClientMock = CreateSMTPClientMock(serviceProvider);

                    //-- ISMTPClientFactory
                    var smtpFactoryMock = CreateSMTPFactoryMock(serviceProvider, smtpClientMock.Object);

                    //-- ICorrelationService
                    var ICorrelationService = CreateCorrelationServiceMock(serviceProvider);

                    //-- IHostApplicationLifetime
                    var hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISMTPService<Test>>();
                    var uutConcrete = (SMTPService<Test>)uut;

                    var taskOne = Task.Delay(50);
                    uutConcrete.smtpClientHandlers.TryAdd(taskOne, "");
    
                    var taskTwo = Task.Delay(100);
                    uutConcrete.smtpClientHandlers.TryAdd(taskTwo, "");
        
                    //Act
                    await uutConcrete.FlushAsync().ConfigureAwait(false);

                    //Assert
                    Assert.IsTrue(uutConcrete.smtpClientHandlers.All(e => e.Key.Status == TaskStatus.RanToCompletion));
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion


        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //--Core
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<SMTPService<Test>>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());
            serviceCollection.AddSingleton(Mock.Of<ISMTPClientFactory>());
            serviceCollection.AddSingleton(Mock.Of<ISmtpClient>());
            serviceCollection.AddSingleton(Mock.Of<ISmtpClient>());

            //--Encryption

            //--Infrastructure
            serviceCollection.AddSingleton<ISMTPService<Test>, SMTPService<Test>>();
            serviceCollection.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            //--Options
            serviceCollection.AddOptions();
            var azureTableServiceOptions = new SMTPServiceOptions<Test>
            {
                EmailTimeout = new TimeSpan(0, 0, 0, 0, 100),
                Host = "SampleHost",
                IdealEmailLoad = 2,
                InactivityTimeout = new TimeSpan(0, 0, 0, 0, 100),
                PullingDelay = new TimeSpan(0, 0, 0, 0,100),
                MaxConnections = 2,
                Password = "SamplePassword",
                Port = 100,
                UserName = "SampleUserName"

            };
            var options = Options.Create(azureTableServiceOptions);
            serviceCollection.AddSingleton<IOptions<SMTPServiceOptions<Test>>>(options);

            var configurationRoot = BuildConfigurationRoot(azureTableServiceOptions);
            serviceCollection.AddSingleton<IConfiguration>(configurationRoot);

            return serviceCollection;
        }

        private Mock<IDateTimeService> CreateDateTimeServiceMock(IServiceProvider serviceProvider, DateTime getDateTimeUTCResponse)
        {
            var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
            dateTimeServiceMock.Setup(stopwatchService => stopwatchService.GetDateTimeUTC()).Returns(getDateTimeUTCResponse);

            return dateTimeServiceMock;
        }

        private Mock<IStopwatchService> CreateStopWatchServiceMock(IServiceProvider serviceProvider)
        {
            var stopwatchServiceMock = serviceProvider.GetMock<IStopwatchService>();

            var stopwatch = new Stopwatch();

            stopwatchServiceMock.Setup
            (
                stopwatchService => stopwatchService.Start()
            ).Callback
            (
                () => { stopwatch.Start(); }
            );

            stopwatchServiceMock.SetupGet
            (
                stopwatchService => stopwatchService.ElapsedMilliseconds
            )
            .Returns
            (
                () => { return stopwatch.ElapsedMilliseconds; }
            );


            stopwatchServiceMock.SetupGet
            (
                stopwatchService => stopwatchService.Elapsed
            )
            .Returns
            (
                () => { return stopwatch.Elapsed; }
            );

            stopwatchServiceMock.Setup
            (
                stopwatchService => stopwatchService.Stop()
            )
            .Callback
            (
                () => { stopwatch.Start(); }
            );

            return stopwatchServiceMock;
        }

        private Mock<IStopwatchFactory> CreateStopWatchFactoryMock(IServiceProvider serviceProvider, IStopwatchService stopwatchService)
        {
            var stopwatchFactoryMock = serviceProvider.GetMock<IStopwatchFactory>();

            stopwatchFactoryMock
            .Setup(stopwatchFactory => stopwatchFactory.NewStopwatchService())
            .Returns(stopwatchService);

            return stopwatchFactoryMock;
        }

        private (Mock<ILoggerService<SMTPService<Test>>>, List<Dictionary<string, object>>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<SMTPService<Test>>>();
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

                if(properties != null)
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
        private (Mock<ITelemetryWriterService>, InsertTelemetryItem) CreateTelemetryWriterServiceMock(IServiceProvider serviceProvider)
        {
            var insertTelemetryRequestObserved = new InsertTelemetryItem();
            var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryWriterService>();
            telemetryServiceWriterMock
            .Setup
            (
                telemetryServiceWriter => telemetryServiceWriter.Insert
                (
                    It.IsAny<InsertTelemetryItem>()
                )
            )
            .Callback((InsertTelemetryItem insertTelemetryRequest) =>
            {
                insertTelemetryRequestObserved.ConnectionName = insertTelemetryRequest.ConnectionName;
                insertTelemetryRequestObserved.CorrelationId = insertTelemetryRequest.CorrelationId;
                insertTelemetryRequestObserved.DateTimeUTC = insertTelemetryRequest.DateTimeUTC;
                insertTelemetryRequestObserved.Duration = insertTelemetryRequest.Duration;
                insertTelemetryRequestObserved.Request = insertTelemetryRequest.Request;
                insertTelemetryRequestObserved.TelemetryResponseState = insertTelemetryRequest.TelemetryResponseState;
                insertTelemetryRequestObserved.TelemetryType = insertTelemetryRequest.TelemetryType;
            });
            return (telemetryServiceWriterMock, insertTelemetryRequestObserved);
        }
        private Mock<ISmtpClient> CreateSMTPClientMock(IServiceProvider serviceProvider, bool sendAsyncShouldThrow = false, bool connectAsyncShouldThrow = false)
        {
            var smtpClientMock = serviceProvider.GetMock<ISmtpClient>();

            if(sendAsyncShouldThrow)
            {
                smtpClientMock.Setup
                (
                    smtpClient => smtpClient.SendAsync
                    (
                        It.IsAny<MimeMessage>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<ITransferProgress>()
                    )
                ).ThrowsAsync(new Exception());
            }

            if (connectAsyncShouldThrow)
            {
                smtpClientMock.Setup
                (
                    smtpClient => smtpClient.ConnectAsync
                    (
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<SecureSocketOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                ).ThrowsAsync(new Exception());
            }

            return smtpClientMock;
        }
        private Mock<ICorrelationService> CreateCorrelationServiceMock(IServiceProvider serviceProvider)
        {
            var correlationService = serviceProvider.GetMock<ICorrelationService>();

            return correlationService;
        }

        private Mock<ISMTPClientFactory> CreateSMTPFactoryMock(IServiceProvider serviceProvider, ISmtpClient smtpClient)
        {
            var smtpClientFactory = serviceProvider.GetMock<ISMTPClientFactory>();

            smtpClientFactory
            .Setup
            (
                smtpClientFactory => smtpClientFactory.Create()
            )
            .Returns(smtpClient);

          
            return smtpClientFactory;
        }


        public async Task SampleMethod()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task SampleMethodThrow()
        {
            await Task.CompletedTask.ConfigureAwait(false); 
            throw new Exception();
        }
        #endregion

        public class Test : SMTPServiceOptionsType
        {
        }
    }

}
