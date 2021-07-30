using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.DNS;
using DickinsonBros.Infrastructure.DNS.Abstractions;
using DickinsonBros.Infrastructure.DNS.Abstractions.Models;
using DickinsonBros.Test.Unit;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos.Tests
{
    [TestClass]
    public class DNSServiceTests : BaseTest
    {

        #region ValidateEmailDomainAsync

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

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
        public async Task ValidateEmailDomainAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

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
        public async Task ValidateEmailDomainAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Runs_LookupClientQueryAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    lookupClientMock
                    .Verify
                    (
                        lookupClient => lookupClient.QueryAsync
                        (
                            domain,
                            QueryType.MX,
                            It.IsAny<QueryClass>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Runs_LogInformationCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(DNSService)}.{nameof(DNSService.ValidateEmailDomainAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var emailDomainObserved = (string)propertiesObserved.First()["emailDomain"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(domain, emailDomainObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_VaildDomain_ReturnsVaild()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(ValidateEmailDomainResult.Vaild, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_InvaildDomain_ReturnsInvaild()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider, false);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(ValidateEmailDomainResult.Invaild, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider, false, true);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(DNSService)}.{nameof(DNSService.ValidateEmailDomainAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var emailDomainObserved = (string)propertiesObserved.First()["emailDomain"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(domain, emailDomainObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateEmailDomainAsync_Throws_ReturnsUnableToReachDNS()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider, false, true);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(ValidateEmailDomainResult.UnableToReachDNS, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }


        [TestMethod]
        public async Task ValidateEmailDomainAsync_Runs_telemetryWriterServiceInsertCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var domain = "SampleDomain.com";

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

                    //--ICreateLookupClientMock
                    var lookupClientMock = CreateLookupClientMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IDNSService>();
                    var uutConcrete = (DNSService)uut;

                    //Act
                    var observed = await uutConcrete.ValidateEmailDomainAsync(domain).ConfigureAwait(false);

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

                    Assert.AreEqual("ILookupClient Domain Servers", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.DomainNameServer, insertTelemetryRequestObserved.TelemetryType);

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
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<IDNSService>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());

            //--Infrastructure
            serviceCollection.AddSingleton<IDNSService, DNSService>();
            serviceCollection.AddSingleton(Mock.Of<ILookupClient>());
 
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
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Start());
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.ElapsedMilliseconds).Returns(0);
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Stop());

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

        private (Mock<ILoggerService<IDNSService>>, List<Dictionary<string, object>>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<IDNSService>>();
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
                foreach (var prop in properties)
                {
                    props.Add(prop.Key, prop.Value);
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

        private Mock<ILookupClient> CreateLookupClientMock(IServiceProvider serviceProvider, bool vaildEmail = true, bool shouldThrow = false)
        {
            var lookupClientMock = serviceProvider.GetMock<ILookupClient>();

            if (shouldThrow)
            {
                lookupClientMock.Setup
                (
                    lookupClient => lookupClient.QueryAsync
                    (
                        It.IsAny<string>(),
                        It.IsAny<QueryType>(),
                        It.IsAny<QueryClass>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync
                (
                   new Exception()
                );

                return lookupClientMock;
            }

            var mxRecordsExpected = new MxRecord[0];

            if(vaildEmail)
            {
                mxRecordsExpected = new[] { new MxRecord(new ResourceRecordInfo("Sample.com", ResourceRecordType.MX, QueryClass.IN, 255, 0), 0, DnsString.RootLabel) };
            }

            var dnsQueryResponseMock = new Mock<IDnsQueryResponse>();

            dnsQueryResponseMock
            .Setup
            (
                dnsQueryResponse => dnsQueryResponse.Answers
            )
            .Returns(mxRecordsExpected);

            lookupClientMock.Setup
            (
                lookupClient => lookupClient.QueryAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<QueryType>(),
                    It.IsAny<QueryClass>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync
            (
               dnsQueryResponseMock.Object
            );

            return lookupClientMock;
        }

        #endregion

    }

}
