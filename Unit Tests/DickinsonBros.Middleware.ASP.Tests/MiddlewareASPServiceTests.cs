using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.MiddlewareService.ASP.Tests
{
    public class StatusCodeTestClass
    {
        public int StatusCode { get; set; }
    }

    [TestClass]
    public class MiddlewareASPServiceTests : BaseTest
    {
        #region InvokeAsnyc
        [TestMethod]
        public async Task InvokeAsync_Runs_RequestLogged()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Logging
                    var logInformationRedactedInvokeCount = 0;
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        logInformationRedactedInvokeCount++;
                        if (logInformationRedactedInvokeCount == 1)
                        {
                            propertiesObserved = properties;
                        }
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();


                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/API/Test";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );
                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            "+ " + expectedURL,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    Assert.AreEqual(requestPath, propertiesObserved["Path"]);
                    Assert.AreEqual("B", ((Dictionary<string, StringValues>)propertiesObserved["Prams"])["A"].ToString());
                    Assert.AreEqual(method, propertiesObserved["Method"]);
                    Assert.AreEqual(scheme, propertiesObserved["Scheme"]);
                    Assert.AreEqual(requestBody, propertiesObserved["Body"]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_Runs_RequestDelegateInvoked()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                        .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()));

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    requestDelegateMock
                        .Verify
                        (requestDelegate =>
                            requestDelegate.Invoke(httpContextMock.Object)
                        );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_Runs_CorrelationIdAddedToResponseHeaders()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                        .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()));

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );
                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    Assert.AreEqual(guidExpected.ToString(), headerDictionaryResponse[MiddlewareASPService.CORRELATION_ID].ToString());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_Runs_ResponseLogged()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedStatusCode = 200;

                    //  Logging
                    var logInformationRedactedInvokeCount = 0;
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        logInformationRedactedInvokeCount++;
                        if (logInformationRedactedInvokeCount == 2)
                        {
                            propertiesObserved = properties;
                        }
                    });

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = expectedStatusCode };
                    var requestPath = "/API/Test";
                    var requestBody = "Test Body";
                    var responseBody = "OK";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                        .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()))
                        .Callback(() =>
                        {
                            HttpResponseMock.Object.StatusCode = responseStatusCode.StatusCode;

                            byte[] byteArrayResponse = Encoding.UTF8.GetBytes(responseBody);
                            HttpResponseMock.Object.Body = new MemoryStream(byteArrayResponse);
                        });
                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            "Response " + expectedURL,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    Assert.AreEqual(expectedStatusCode, propertiesObserved["StatusCode"]);
                    Assert.AreEqual(responseBody, propertiesObserved["Body"]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_RequestDelegateThrows_StatusCodeSetToServerError()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedException = new Exception("Delegate Failure");

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogErrorRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, Exception, IDictionary<string, object>>((message, logGroup, exception, properties) =>
                    {
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()))
                .ThrowsAsync(expectedException);

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    Assert.AreEqual(500, HttpResponseMock.Object.StatusCode);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_RequestDelegateThrows_CorrelationIdAddedToResponseHeaders()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedException = new Exception("Delegate Failure");

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogErrorRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, Exception, IDictionary<string, object>>((message, logGroup, exception, properties) =>
                    {
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()))
                .ThrowsAsync(expectedException);

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );


                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    Assert.AreEqual(guidExpected.ToString(), headerDictionaryResponse[MiddlewareASPService.CORRELATION_ID].ToString());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_RequestDelegateThrows_LogError()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var expectedException = new Exception("Delegate Failure");

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Logging
                    var expectedStatusCode = 500;
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
                    (
                        loggingService => loggingService.LogErrorRedacted
                        (
                            It.IsAny<string>(),
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, Exception, IDictionary<string, object>>((message, logGroup, exception, properties) =>
                    {
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = expectedStatusCode };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    //  Request Delegate 
                    var requestDelegateMock = serviceProvider.GetMock<RequestDelegate>();
                    requestDelegateMock
                .Setup(requestDelegate => requestDelegate.Invoke(It.IsAny<HttpContext>()))
                .ThrowsAsync(expectedException);

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogErrorRedacted
                        (
                            "Unhandled exception " + expectedURL,
                            LogGroup.Infrastructure,
                            expectedException,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    Assert.AreEqual(expectedStatusCode, propertiesObserved["StatusCode"]);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_Runs_ExitLogged()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    loggingServiceMock.Verify
                    (
                        loggingService => loggingService.LogInformationRedacted
                        (
                            "- " + expectedURL,
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    Assert.AreEqual(100, ((TimeSpan)propertiesObserved["Duration"]).TotalMilliseconds);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_StatusCodeLessThen200_AddsSuccessfulTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 199 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    telemetryServiceMock.Verify
                    (
                        loggingService => loggingService.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual(expectedURL, telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Application, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_StatusCode2xx_AddsSuccessfulTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 200 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    telemetryServiceMock.Verify
                    (
                        loggingService => loggingService.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual(expectedURL, telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Application, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_StatusCode201_AddsSuccessfulTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 201 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    telemetryServiceMock.Verify
                    (
                        loggingService => loggingService.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual(expectedURL, telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Application, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_StatusCode4xx_AddsBadRequestTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 400 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    telemetryServiceMock.Verify
                    (
                        loggingService => loggingService.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual(expectedURL, telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.CallerError, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Application, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InvokeAsync_StatusCodeNot2xxOr4xx_AddsFailureTelemetry()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);


                    //  Logging
                    var propertiesObserved = (IDictionary<string, object>)null;
                    var loggingServiceMock = serviceProvider.GetMock<ILoggerService<MiddlewareASPService>>();
                    loggingServiceMock.Setup
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
                        propertiesObserved = properties;
                    });

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpContext 
                    var httpContextMock = new Mock<HttpContext>();
                    var httpRequestMock = new Mock<HttpRequest>();
                    var HttpResponseMock = new Mock<HttpResponse>();

                    var headers = new Dictionary<string, string>();
                    var headerDictionaryResponse = new HeaderDictionary();
                    StatusCodeTestClass responseStatusCode = new StatusCodeTestClass { StatusCode = 500 };
                    var requestPath = "/APITest";
                    var requestBody = "Test Body";
                    var requestQuery = new QueryCollection
                    (
                        new Dictionary<string, StringValues>
                        {
                            {"A", "B" }
                        }
                    );

                    var method = "GET";
                    var scheme = "https";
                    var host = "testhost";
                    var expectedURL = $"{method} {scheme}://{host}{requestPath}";
                    ConfigureHttpContext
                    (
                        httpContextMock,
                        httpRequestMock,
                        HttpResponseMock,
                        headers,
                        method,
                        scheme,
                        host,
                        requestBody,
                        responseStatusCode,
                        requestPath,
                        requestQuery,
                        headerDictionaryResponse
                    );

                    //  DateTime
                    var dateTimeExpected = new System.DateTime(2020, 1, 1);
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(dateTimeExpected);

                    stopwatchServiceMock
                    .Setup(stopwatchService => stopwatchService.Start());

                    stopwatchServiceMock
                    .Setup(stopwatchService => stopwatchService.Stop());

                    //  Telemetry
                    var telemetryDataObserved = (InsertTelemetryItem)null;
                    var telemetryServiceMock = serviceProvider.GetMock<ITelemetryWriterService>();
                    telemetryServiceMock
                        .Setup(telemetryService => telemetryService.Insert(It.IsAny<InsertTelemetryItem>()))
                        .Callback<InsertTelemetryItem>((telemetryData) =>
                        {
                            telemetryDataObserved = telemetryData;
                        });

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    await uut.InvokeAsync(httpContextMock.Object);

                    //Assert
                    telemetryServiceMock.Verify
                    (
                        loggingService => loggingService.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(dateTimeExpected, telemetryDataObserved.DateTimeUTC);
                    Assert.AreEqual(100, telemetryDataObserved.Duration.TotalMilliseconds);
                    Assert.AreEqual(expectedURL, telemetryDataObserved.Request);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryDataObserved.TelemetryResponseState);
                    Assert.AreEqual(TelemetryType.Application, telemetryDataObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region EnsureCorrelationId
        [TestMethod]
        public async Task EnsureCorrelationId_ContainsCorrelationIdInHeader_ReturnsCorrelationIdFromHeader()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    await Task.CompletedTask;

                    var expected = "1000";
                    var httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                       .SetupGet(httpRequest => httpRequest.Headers)
                       .Returns(() =>
                       {
                           var headerDictionary = new HeaderDictionary
                           {
                               { MiddlewareASPService.CORRELATION_ID, expected }
                           };

                           return headerDictionary;
                       });

                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    var observed = uut.EnsureCorrelationId(httpRequestMock.Object);

                    //Assert
                    Assert.AreEqual(expected, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task EnsureCorrelationId_DoesNotContainsCorrelationIdInHeader_ReturnsNewGuid()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    await Task.CompletedTask;

                    //  Guid 
                    var guidExpected = new System.Guid("10000000-0000-0000-0000-000000000000");
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid())
                        .Returns(() => guidExpected);

                    //  HttpRequest
                    var httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                       .SetupGet(httpRequest => httpRequest.Headers)
                       .Returns(() =>
                       {
                           var headerDictionary = new HeaderDictionary
                           {
                           };

                           return headerDictionary;
                       });

                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    var observed = uut.EnsureCorrelationId(httpRequestMock.Object);

                    //Assert
                    Assert.AreEqual(guidExpected.ToString(), observed);
                    guidServiceMock
                        .Verify(guidService => guidService.NewGuid());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region Helpers

        #region FormatRequestAsync
        [TestMethod]
        public async Task FormatRequestAsync_Runs_ReturnsStringAndSetsStreamToStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var expected = "Test String";

                    byte[] byteArrayRequest = Encoding.UTF8.GetBytes(expected);
                    MemoryStream requestBodyStream = new MemoryStream(byteArrayRequest);

                    var httpRequestMock = new Mock<HttpRequest>();
                    httpRequestMock
                       .SetupGet(HttpRequest => HttpRequest.Body)
                       .Returns(() => requestBodyStream);

                    httpRequestMock
                       .SetupGet(HttpRequest => HttpRequest.ContentLength)
                       .Returns(() => byteArrayRequest.Length);

                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    var observed = await uut.FormatRequestAsync(httpRequestMock.Object);

                    //Assert
                    Assert.AreEqual(expected, observed);
                    Assert.AreEqual(0, requestBodyStream.Position);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region FormatResponseAsync
        [TestMethod]
        public async Task FormatResponseAsync_Runs_ReturnsString()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var expected = "Test String";

                    byte[] byteArrayRequest = Encoding.UTF8.GetBytes(expected);
                    MemoryStream requestBodyStream = new MemoryStream(byteArrayRequest);

                    var httpResponseMock = new Mock<HttpResponse>();
                    httpResponseMock
                   .SetupGet(HttpResponse => HttpResponse.Body)
                   .Returns(() => requestBodyStream);

                    //  Unit Under Test
                    var uut = serviceProvider.GetRequiredService<MiddlewareASPService>();

                    //Act
                    var observed = await uut.FormatResponseAsync(httpResponseMock.Object);

                    //Assert
                    Assert.AreEqual(expected, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion
        
        private void ConfigureHttpContext
        (
            Mock<HttpContext> httpContextMock,
            Mock<HttpRequest> httpRequestMock,
            Mock<HttpResponse> httpResponseMock,
            Dictionary<string, string> headers,
            string requestMethod,
            string requestScheme,
            string requestHost,
            string requestBody,
            StatusCodeTestClass responseStatusCode,
            string requestPath,
            QueryCollection RequestQuery,
            HeaderDictionary ResponseHeaders
        )
        {
            byte[] byteArrayRequest = Encoding.UTF8.GetBytes(requestBody);
            MemoryStream requestBodyStream = new MemoryStream(byteArrayRequest);

            Stream ResponseStream = new MemoryStream();

            httpContextMock
                .SetupGet(httpContext => httpContext.Request)
                .Returns(() => httpRequestMock.Object);

            httpContextMock
                .SetupGet(httpContext => httpContext.Response)
                .Returns(() => httpResponseMock.Object);

            httpContextMock
                .SetupGet(httpContext => httpContext.Request)
                .Returns(() => httpRequestMock.Object);

            httpRequestMock
                .SetupGet(httpRequestMock => httpRequestMock.Path)
                .Returns(() => requestPath);

            httpRequestMock
            .SetupGet(httpRequestMock => httpRequestMock.Method)
            .Returns(() => requestMethod);

            httpRequestMock
            .SetupGet(httpRequestMock => httpRequestMock.Scheme)
            .Returns(() => requestScheme);


            httpRequestMock
            .SetupGet(httpRequestMock => httpRequestMock.Host)
            .Returns(() => new HostString(requestHost));

            httpRequestMock
              .SetupGet(httpRequestMock => httpRequestMock.ContentLength)
              .Returns(() => byteArrayRequest.Length);

            httpRequestMock
                .SetupGet(httpRequestMock => httpRequestMock.Query)
                .Returns(() => RequestQuery);

            httpRequestMock
                .SetupGet(httpRequest => httpRequest.Body)
                .Returns(() => requestBodyStream);

            httpRequestMock
                .SetupGet(httpRequest => httpRequest.Headers)
                .Returns(() =>
                {
                    var headerDictionary = new HeaderDictionary();

                    foreach (var header in headers)
                    {
                        headerDictionary.Add(header.Key, header.Value);
                    }
                    return headerDictionary;
                });

            httpResponseMock
                .SetupGet(HttpResponse => HttpResponse.Body)
                .Returns(() => ResponseStream);

            httpResponseMock
                .SetupSet(HttpResponse => HttpResponse.Body = It.IsAny<Stream>())
                .Callback<Stream>(stream => ResponseStream = stream);

            httpResponseMock
                .SetupGet(HttpResponse => HttpResponse.Headers)
                .Returns(() => ResponseHeaders);

            httpResponseMock
                .SetupGet(HttpResponse => HttpResponse.StatusCode)
                .Returns(() => responseStatusCode.StatusCode);

            httpResponseMock
                .SetupSet(HttpResponse => HttpResponse.StatusCode = It.IsAny<int>())
                .Callback<int>(statusCode => responseStatusCode.StatusCode = statusCode);
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MiddlewareASPService>();
            serviceCollection.AddSingleton(Mock.Of<RequestDelegate>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<MiddlewareASPService>>());
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());

            return serviceCollection;
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
        #endregion
    }
}