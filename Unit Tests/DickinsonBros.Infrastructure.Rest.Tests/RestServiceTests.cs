using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.Infrastructure.Rest.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Rest.Tests
{
    [TestClass]
    public class RestServiceTests : BaseTest
    {
        public const string ATTEMPTS = "Attempts";
        public const string BASEURL = "BaseUrl";
        public const string RESOURCE = "Resource";
        public const string BODY = "Body";
        public const string REQUEST_CONTENT = "RequestContent";
        public const string RESPONSE_CONTENT = "ResponseContent";
        public const string ELAPSED_MILLISECONDS = "ElapsedMilliseconds";
        public const string STATUS_CODE = "StatusCode";

        #region ExecuteAsync

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_NullRequest_ThrowException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //HTTP
                    var httpRequestMessage = (HttpRequestMessage)null;
                    var retrys = 0;
                    var timeout = 30.0;

                    var httpClientMock = new HttpClient()
                    {
                    };

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                   
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_Runs_CorrelationServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;
                    var expectedGuid = new System.Guid("2cce9f60-a582-480d-a0ea-4ea39dab6961");

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                    };

                    httpRequestMessage.Headers.Add(RestService.CORRELATION_ID, "DemoId");

                    var httpResponseMessage = new HttpResponseMessage();

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  Correlation
                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                        .SetupGet(correlationService => correlationService.CorrelationId);

                    //  Guid
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid());


                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    correlationServiceMock
                    .VerifyGet(
                        correlationService => correlationService.CorrelationId,
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_VaildInput_LogsRequestRedacted()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);


                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            messagesObserved[index],
                            LogGroup.Infrastructure,
                            propertiesObserved[index]
                        )
                    );
                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_VaildInput_StopWatchStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage();

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    stopwatchServiceMock
                    .Verify(
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_VaildInput_RestClientExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage();

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    httpMessageHandlerMock.Protected().Verify(
                       "SendAsync",
                       Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req == httpRequestMessage),
                       ItExpr.IsAny<CancellationToken>());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_VaildInput_StopWatchStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage();

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    stopwatchServiceMock
                    .Verify(
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_Timeout_Retry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 2;
                    var timeout = 0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ThrowsAsync(new OperationCanceledException());

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    //  Logging
                    string messageObserved = null;

                    Dictionary<string, object> propertiesObserved = null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<LogGroup>(),
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messageObserved = message;

                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    Assert.AreEqual(3, (int)propertiesObserved[ATTEMPTS]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_ResponseIsSuccessful_LogResponseRedacted()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);

                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            messagesObserved[index],
                            LogGroup.Infrastructure,
                            propertiesObserved[index]
                        )
                    );

                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), propertiesObserved[index][RESPONSE_CONTENT]);
                    Assert.IsTrue((int)propertiesObserved[index][ELAPSED_MILLISECONDS] >= 0);
                    Assert.AreEqual(httpResponseMessage.StatusCode, propertiesObserved[index][STATUS_CODE]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_ResponseIsNotSuccessful_LogResponseRedactedAsError()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.InternalServerError,
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                It.IsAny<LogGroup>(),

                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);

                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            messagesObserved[index],
                            LogGroup.Infrastructure,
                            propertiesObserved[index]
                        )
                    );

                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), propertiesObserved[index][RESPONSE_CONTENT]);
                    Assert.IsTrue((int)propertiesObserved[index][ELAPSED_MILLISECONDS] >= 0);
                    Assert.AreEqual(httpResponseMessage.StatusCode, propertiesObserved[index][STATUS_CODE]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_FailedAndRetrys_AttemptsExpected()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 2;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    //  Logging
                    string messageObserved = null;

                    Dictionary<string, object> propertiesObserved = null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(), 
                                LogGroup.Infrastructure,
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messageObserved = message;
                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    Assert.AreEqual(3, (int)propertiesObserved[ATTEMPTS]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsync_Runs_InsertTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {

                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent("{\"name\":\"Same Doe\",\"age\":35}", Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    //  Logging
                    string messageObserved = null;
                    Dictionary<string, object> propertiesObserved = null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,

                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messageObserved = message;
                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    telemetryServiceMock
                    .Verify(
                        telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual($"{httpRequestMessage.Method.Method}", telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Rest, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region ExecuteAsyncOfT

        [TestMethod]
        public async Task ExecuteAsyncOfT_VaildInput_LogsRequestRedacted()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);


                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            messagesObserved[index],
                            LogGroup.Infrastructure,
                            propertiesObserved[index]
                        )
                    );
                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public async Task ExecuteAsyncOfT_VaildInput_StopWatchStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    stopwatchServiceMock
                    .Verify(
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsyncOfT_VaildInput_RestClientExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    httpMessageHandlerMock.Protected().Verify(
                       "SendAsync",
                       Times.Exactly(1),
                       ItExpr.Is<HttpRequestMessage>(req => req == httpRequestMessage),
                       ItExpr.IsAny<CancellationToken>());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsyncOfT_VaildInput_StopWatchStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative)
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    //  Logging
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    stopwatchServiceMock
                    .Verify(
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsyncOfT_ResponseIsSuccessful_LogResponseRedacted()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogInformationRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,
                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);

                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            messagesObserved[index], 
                            LogGroup.Infrastructure,
                            propertiesObserved[index]
                        )
                    );

                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), propertiesObserved[index][RESPONSE_CONTENT]);
                    Assert.IsTrue((int)propertiesObserved[index][ELAPSED_MILLISECONDS] >= 0);
                    Assert.AreEqual(httpResponseMessage.StatusCode, propertiesObserved[index][STATUS_CODE]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsyncOfT_ResponseIsNotSuccessful_LogResponseRedactedAsError()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };

                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.InternalServerError,
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var messagesObserved = new List<string>();
                    var propertiesObserved = new List<Dictionary<string, object>>();
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,

                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messagesObserved.Add(message);
                            propertiesObserved.Add((Dictionary<string, object>)properties);
                        });

                    //  DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));


                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    var index = messagesObserved.FindIndex(message => message == RestService.RESULT);

                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogWarningRedacted
                        (
                            messagesObserved[index], 
                            LogGroup.Infrastructure,
                            
                            propertiesObserved[index]

                        )
                    );

                    Assert.AreEqual(1, messagesObserved.Where(message => message == RestService.RESULT).Count());
                    Assert.AreEqual(RestService.RESULT, messagesObserved[index]);
                    Assert.AreEqual(httpClientMock.BaseAddress, propertiesObserved[index][BASEURL].ToString());
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), propertiesObserved[index][REQUEST_CONTENT]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), propertiesObserved[index][RESPONSE_CONTENT]);
                    Assert.IsTrue((int)propertiesObserved[index][ELAPSED_MILLISECONDS] >= 0);
                    Assert.AreEqual(httpResponseMessage.StatusCode, propertiesObserved[index][STATUS_CODE]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public async Task ExecuteAsyncOfT_Runs_InsertTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;

                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };
                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    //  Logging
                    string messageObserved = null;

                    Dictionary<string, object> propertiesObserved = null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,

                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messageObserved = message;

                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    telemetryServiceMock
                    .Verify(
                        telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual($"{httpRequestMessage.Method.Method}", telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Rest, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ExecuteAsyncOfT_Runs_ReturnsDataAsContentDeserialized()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Prams
                    var retrys = 0;
                    var timeout = 30.0;




                    //HTTP
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json")
                    };
                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json")
                    };

                    var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                    httpMessageHandlerMock
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>(
                      "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(httpResponseMessage);

                    var httpClientMock = new HttpClient(httpMessageHandlerMock.Object)
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };

                    var expectedResponse = new HttpResponse<DataClass>
                    {
                        HttpResponseMessage = httpResponseMessage,
                        Data = JsonSerializer.Deserialize<DataClass>(
                            await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false),
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            }
                        )
                    };

                    //  Logging
                    string messageObserved = null;

                    Dictionary<string, object> propertiesObserved = null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
                    loggingServiceMock
                        .Setup
                        (
                            loggingService => loggingService.LogWarningRedacted
                            (
                                It.IsAny<string>(),
                                LogGroup.Infrastructure,

                                It.IsAny<IDictionary<string, object>>()
                            )
                        )
                        .Callback<string, LogGroup, IDictionary<string, object>>((message, logGroup, properties) =>
                        {
                            messageObserved = message;

                            propertiesObserved = (Dictionary<string, object>)properties;
                        });

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //   Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.ExecuteAsync<DataClass>("SampleConnectionName", httpClientMock, httpRequestMessage, retrys, timeout);

                    //Assert
                    telemetryServiceMock
                    .Verify(
                        telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()),
                        Times.Once
                    );

                    Assert.AreEqual(observed.HttpResponseMessage, observed.HttpResponseMessage);
                    Assert.AreEqual(observed.Data, observed.Data);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion
        #region DetermineTelemetryResponseState

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCodeLessThen200_ReturnUnhandledException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    
                    //--Input
                    var statusCode = 100;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCode2xx_ReturnSuccessful()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var statusCode = 200;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.Successful, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCode3xx_ReturnUnhandledException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var statusCode = 300;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCode4xx_ReturnCallerError()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var statusCode = 400;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.CallerError, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCode5xx_ReturnUnhandledException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var statusCode = 500;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DetermineTelemetryResponseState_StatusCode6xx_ReturnUnhandledException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var statusCode = 600;

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.DetermineTelemetryResponseState(statusCode);

                    //Assert
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region LogResult

        [TestMethod]
        public async Task LogResult_IsNotSuccessStatusCode_LogWarningRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent
                        (
@"{
  ""Sample"": 1,
}"                          ,
                            Encoding.UTF8,
                            "application/json"
                         ),
                    };
                    var httpResponseMessage = new HttpResponseMessage
                    {
                        Content = new StringContent
                        (
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                         ,
                            Encoding.UTF8,
                            "application/json"
                         ),
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    var attempts = 1;
                    int elapsedMilliseconds = 1;

                    //--ILogger
                    var (loggerServiceMock, propertiesObserved) = CreateWarningLoggerService(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.LogResult(httpRequestMessage, httpResponseMessage, httpClient, attempts, elapsedMilliseconds);

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogWarningRedacted
                        (
                            RestService.RESULT,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(attempts, (int)propertiesObserved.First()["Attempts"]);
                    Assert.AreEqual(httpClient.BaseAddress.ToString(), ((Uri)propertiesObserved.First()["BaseUrl"]).ToString());
                    Assert.AreEqual(httpRequestMessage.RequestUri.OriginalString, (string)propertiesObserved.First()["Resource"]);
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), (string)propertiesObserved.First()["RequestContent"]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), (string)propertiesObserved.First()["ResponseContent"]);
                    Assert.AreEqual(elapsedMilliseconds, (int)propertiesObserved.First()["ElapsedMilliseconds"]);
                    Assert.AreEqual(httpResponseMessage.StatusCode, (HttpStatusCode)propertiesObserved.First()["StatusCode"]);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LogResult_HttpResponseMessageIsNull_LogWarningRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent
                        (
@"{
  ""Sample"": 1,
}",
                            Encoding.UTF8,
                            "application/json"
                         ),
                    };
                    var httpResponseMessage = (HttpResponseMessage)null;
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    var attempts = 1;
                    int elapsedMilliseconds = 1;

                    //--ILogger
                    var (loggerServiceMock, propertiesObserved) = CreateWarningLoggerService(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.LogResult(httpRequestMessage, httpResponseMessage, httpClient, attempts, elapsedMilliseconds);

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogWarningRedacted
                        (
                            RestService.RESULT,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(attempts, (int)propertiesObserved.First()["Attempts"]);
                    Assert.AreEqual(httpClient.BaseAddress.ToString(), ((Uri)propertiesObserved.First()["BaseUrl"]).ToString());
                    Assert.AreEqual(httpRequestMessage.RequestUri.OriginalString, (string)propertiesObserved.First()["Resource"]);
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), (string)propertiesObserved.First()["RequestContent"]);
                    Assert.IsNull(propertiesObserved.First()["ResponseContent"]);
                    Assert.AreEqual(elapsedMilliseconds, (int)propertiesObserved.First()["ElapsedMilliseconds"]);
                    Assert.IsNull(propertiesObserved.First()["StatusCode"]);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task LogResult_IsSuccessStatusCodeAndHttpResponseIsNotNull_LogWarningRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent
                       (
@"{
  ""Sample"": 1,
}",
                           Encoding.UTF8,
                           "application/json"
                        ),
                    };
                    var httpResponseMessage = new HttpResponseMessage
                    {
                        Content = new StringContent
                        (
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                         ,
                            Encoding.UTF8,
                            "application/json"
                         ),
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
                    };
                    var attempts = 1;
                    int elapsedMilliseconds = 1;

                    //--ILogger
                    var (loggerServiceMock, propertiesObserved) = CreateInformationLoggerService(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = uutConcrete.LogResult(httpRequestMessage, httpResponseMessage, httpClient, attempts, elapsedMilliseconds);

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            RestService.RESULT,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(attempts, (int)propertiesObserved.First()["Attempts"]);
                    Assert.AreEqual(httpClient.BaseAddress.ToString(), ((Uri)propertiesObserved.First()["BaseUrl"]).ToString());
                    Assert.AreEqual(httpRequestMessage.RequestUri.OriginalString, (string)propertiesObserved.First()["Resource"]);
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync(), (string)propertiesObserved.First()["RequestContent"]);
                    Assert.AreEqual(await httpResponseMessage.Content.ReadAsStringAsync(), (string)propertiesObserved.First()["ResponseContent"]);
                    Assert.AreEqual(elapsedMilliseconds, (int)propertiesObserved.First()["ElapsedMilliseconds"]);
                    Assert.AreEqual(httpResponseMessage.StatusCode, (HttpStatusCode)propertiesObserved.First()["StatusCode"]);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region CloneAsnyc

        [TestMethod]
        public async Task CloneAsync_HttpRequestMessage_ReturnsCloned()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    var httpRequestMessage = new HttpRequestMessage()
                    {
                        RequestUri = new Uri("todos/", UriKind.Relative),
                        Content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}"
                        , Encoding.UTF8, "application/json"),

                    };

                    httpRequestMessage.Headers.Add("1-Headers", "1Headers");
                    httpRequestMessage.Headers.Add("2-Headers", "2Headers");
                    httpRequestMessage.Properties.Add("1-Properties", "1Properties");
                    httpRequestMessage.Properties.Add("2-Properties", "2Properties");
                    //  Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.CloneAsync(httpRequestMessage).ConfigureAwait(false);
                    //Assert
                    Assert.AreEqual(await httpRequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false), await observed.Content.ReadAsStringAsync().ConfigureAwait(false));
                    Assert.AreEqual(httpRequestMessage.Version, observed.Version);
                    Assert.AreEqual(httpRequestMessage.Properties.Count, observed.Properties.Count);
                    Assert.AreEqual(httpRequestMessage.Properties["1-Properties"], observed.Properties["1-Properties"]);
                    Assert.AreEqual(httpRequestMessage.Properties["2-Properties"], observed.Properties["2-Properties"]);
                    Assert.AreEqual(httpRequestMessage.Headers.Count(), observed.Headers.Count());
                    Assert.AreEqual(httpRequestMessage.Headers.First(e => e.Key == "1-Headers").Value.First(), observed.Headers.First(e => e.Key == "1-Headers").Value.First());
                    Assert.AreEqual(httpRequestMessage.Headers.First(e => e.Key == "2-Headers").Value.First(), observed.Headers.First(e => e.Key == "2-Headers").Value.First());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CloneAsync_HttpContent_ReturnsCloned()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    var content = new StringContent(
@"{
  ""userId"": 0,
  ""id"": 0,
  ""title"": null,
  ""completed"": false
}");
                    content.Headers.Add("1-Headers", "1Headers");
                    //  Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.CloneAsync(content).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(await content.ReadAsStringAsync().ConfigureAwait(false), await observed.ReadAsStringAsync().ConfigureAwait(false));
                    Assert.AreEqual(content.Headers.First(e => e.Key == "1-Headers").Value.First(), observed.Headers.First(e => e.Key == "1-Headers").Value.First());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task CloneAsync_NullContent_ReturnsNull()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    var content = (HttpContent)null;

                    //  Rest Service
                    var uut = serviceProvider.GetRequiredService<IRestService>();
                    var uutConcrete = (RestService)uut;

                    //Act
                    var observed = await uutConcrete.CloneAsync(content).ConfigureAwait(false);

                    //Assert
                    Assert.IsNull(observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //--Core
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<RestService>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());

            //--Infrastructure
            serviceCollection.AddSingleton<IRestService, RestService>();

            return serviceCollection;
        }

        private (Mock<ILoggerService<RestService>>, List<Dictionary<string, object>>) CreateInformationLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
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

            return (loggerServiceMock, propertiesObserved);
        }
        private (Mock<ILoggerService<RestService>>, List<Dictionary<string, object>>) CreateWarningLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<RestService>>();
            var propertiesObserved = new List<Dictionary<string, object>>();

            loggerServiceMock
            .Setup
            (
                loggerService => loggerService.LogWarningRedacted
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

            return (loggerServiceMock, propertiesObserved);
        }

        private Mock<IStopwatchService> CreateStopWatchServiceMock(IServiceProvider serviceProvider)
        {
            var stopwatchServiceMock = serviceProvider.GetMock<IStopwatchService>();
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Start());
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.ElapsedMilliseconds).Returns(100);
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Elapsed).Returns(new TimeSpan(0, 0, 0, 0, 100));
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
        public class DataClass
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
        }

        #endregion

    }
}
