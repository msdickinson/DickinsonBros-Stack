using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos.Tests
{
    [TestClass]
    public class CosmosServiceTests : BaseTest
    {
        private const string CONNECTION_STRING = "SampleConnectionString";

        #region QueryAsync

        [TestMethod]
        public async Task QueryAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

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
        public async Task QueryAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

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
        public async Task QueryAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

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
        public async Task QueryAsync_Runs_GetItemQueryIteratorCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.GetItemQueryIterator<SampleModel>
                        (
                            It.IsAny<QueryDefinition>(),
                            It.IsAny<string>(),
                            It.IsAny<QueryRequestOptions>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

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
        public async Task QueryAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.QueryAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var queryDefinitionObserved = (QueryDefinition)propertiesObserved.First()["queryDefinition"];
                    var queryRequestOptionsObserved = (QueryRequestOptions)propertiesObserved.First()["queryRequestOptions"];
                    var itemsObserved = (List<SampleModel>)propertiesObserved.First()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(queryDefinition, queryDefinitionObserved);
                    Assert.AreEqual(queryRequestOptions, queryRequestOptionsObserved);
                    Assert.AreEqual(sampleModels.Count(), itemsObserved.Count());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Runs_ReturnsIEnumerableOfT()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(sampleModels[0], observed.ToList()[0]);
                    Assert.AreEqual(sampleModels[1], observed.ToList()[1]);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.QueryAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var queryDefinitionObserved = (QueryDefinition)propertiesObserved.First()["queryDefinition"];
                    var queryRequestOptionsObserved = (QueryRequestOptions)propertiesObserved.First()["queryRequestOptions"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(queryDefinition, queryDefinitionObserved);
                    Assert.AreEqual(queryRequestOptions, queryRequestOptionsObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var queryDefinition = new QueryDefinition("SampleQuery");
                    var queryRequestOptions = new QueryRequestOptions();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync<SampleModel>(queryDefinition, queryRequestOptions).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region FetchAsync

        [TestMethod]
        public async Task FetchAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task FetchAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task FetchAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task FetchAsync_Runs_ReadItemAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.ReadItemAsync<SampleModel>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task FetchAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.FetchAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var idObserved = (string)propertiesObserved.First()["id"];
                    var keyObserved = (string)propertiesObserved.First()["key"];
                    var responseObserved = (ItemResponse<SampleModel>)propertiesObserved.First()["response"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(id, idObserved);
                    Assert.AreEqual(key, keyObserved);
                    Assert.AreEqual(sampleModel, responseObserved.Resource);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_Runs_ReturnsItemResponseOfT()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(sampleModel, observed.Resource);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_ThrowsNotFound_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, false, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<CosmosException>(async () => await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.FetchAsync)}<{typeof(SampleModel).Name}> NotFound",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );
                    var idObserved = (string)propertiesObserved.First()["id"];
                    var keyObserved = (string)propertiesObserved.First()["key"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(id, idObserved);
                    Assert.AreEqual(key, keyObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.NotFound, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_ThrowsNotFound_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, false, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<CosmosException>(async () => await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.FetchAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var idObserved = (string)propertiesObserved.First()["id"];
                    var keyObserved = (string)propertiesObserved.First()["key"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(id, idObserved);
                    Assert.AreEqual(key, keyObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FetchAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleModel>(id, key).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task DeleteAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task DeleteAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task DeleteAsync_Runs_DeleteItemAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.DeleteItemAsync<SampleModel>
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

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
        public async Task DeleteAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.DeleteAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var idObserved = (string)propertiesObserved.First()["id"];
                    var keyObserved = (string)propertiesObserved.First()["key"];
                    var responseObserved = (ItemResponse<SampleModel>)propertiesObserved.First()["response"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(id, idObserved);
                    Assert.AreEqual(key, keyObserved);
                    Assert.AreEqual(sampleModel, responseObserved.Resource);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_ReturnsItemResponseOfT()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(sampleModel, observed.Resource);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.DeleteAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var idObserved = (string)propertiesObserved.First()["id"];
                    var keyObserved = (string)propertiesObserved.First()["key"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(id, idObserved);
                    Assert.AreEqual(key, keyObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var id = "sampleId";
                    var key = "sampleKey";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync<SampleModel>(id, key).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public async Task InsertAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData",
                        Key = "sampleKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");

                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task InsertAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData",
                        Key = "sampleKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task InsertAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task InsertAsync_Runs_CreateItemAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.CreateItemAsync<SampleModel>
                        (
                            It.IsAny<SampleModel>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task InsertAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.InsertAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var sampleModelObserved = (SampleModel)propertiesObserved.First()["value"];
                    var responseObserved = (ItemResponse<SampleModel>)propertiesObserved.First()["response"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModel, sampleModelObserved);
                    Assert.AreEqual(sampleModel, responseObserved.Resource);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertAsync_Runs_ReturnsItemResponseOfT()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(sampleModel, observed.Resource);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.InsertAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var sampleModelObserved = (SampleModel)propertiesObserved.First()["value"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModel, sampleModelObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region UpsertAsync

        [TestMethod]
        public async Task UpsertAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");

                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task UpsertAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task UpsertAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task UpsertAsync_Runs_UpsertItemAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.UpsertItemAsync<SampleModel>
                        (
                            It.IsAny<SampleModel>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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
        public async Task UpsertAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.UpsertAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var sampleModelObserved = (SampleModel)propertiesObserved.First()["value"];
                    var responseObserved = (ItemResponse<SampleModel>)propertiesObserved.First()["response"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModel, sampleModelObserved);
                    Assert.AreEqual(sampleModel, responseObserved.Resource);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_ReturnsItemResponseOfT()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(sampleModel, observed.Resource);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_ThrowsPreconditionFailed_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, false, false, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<CosmosException>(async () => await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.UpsertAsync)}<{typeof(SampleModel).Name}> PreconditionFailed",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var sampleModelObserved = (SampleModel)propertiesObserved.First()["value"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModel, sampleModelObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Conflict, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_ThrowsPreconditionFailed_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, false, false, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<CosmosException>(async () => await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.UpsertAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var sampleModelObserved = (SampleModel)propertiesObserved.First()["value"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModel, sampleModelObserved);
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };
                    


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync<SampleModel>(sampleModel).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region UpsertBulkAsync

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");

                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task UpsertBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task UpsertBulkAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task UpsertBulkAsync_Runs_UpsertItemStreamAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.UpsertItemStreamAsync
                        (
                            It.IsAny<Stream>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Exactly(2)
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task UpsertBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.UpsertBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.Last()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.Last()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_ReturnsIEnumerableOfResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.UpsertBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region InsertBulkAsync

        [TestMethod]
        public async Task InsertBulkAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");

                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task InsertBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task InsertBulkAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task InsertBulkAsync_Runs_CreateItemStreamAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.CreateItemStreamAsync
                        (
                            It.IsAny<Stream>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Exactly(2)
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task InsertBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.InsertBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.Last()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.Last()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_ReturnsIEnumerableOfResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.InsertBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertBulkAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region DeleteBulkAsync

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_GetDateTimeUTCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");

                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task DeleteBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task DeleteBulkAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task DeleteBulkAsync_Runs_DeleteItemStreamAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    containerMock
                    .Verify
                    (
                        container => container.DeleteItemStreamAsync
                        (
                            It.IsAny<string>(),
                            It.IsAny<PartitionKey>(),
                            It.IsAny<ItemRequestOptions>(),
                            It.IsAny<CancellationToken>()
                        ),
                        Times.Exactly(2)
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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
        public async Task DeleteBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };


                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.DeleteBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.Last()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.Last()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.Successful, TelemetryResponseStateObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_ReturnsIEnumerableOfResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(ICosmosService<Test>)}<{typeof(Test).Name}>.{nameof(ICosmosService<Test>.DeleteBulkAsync)}<{typeof(SampleModel).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var itemsObserved = (IEnumerable<SampleModel>)propertiesObserved.Last()["items"];
                    var durationObserved = (TimeSpan)propertiesObserved.First()["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved.First()["TelemetryResponseState"];

                    Assert.AreEqual(sampleModels.First(), itemsObserved.First());
                    Assert.IsNotNull(durationObserved);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, TelemetryResponseStateObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Throws_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert


                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--Input
                    var sampleModel = new SampleModel
                    {
                        SampleData = "SampleSampleData"
                    };



                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cosmos Client and Factory 
                    var uri = new Uri("http://services.odata.org");
                    var sampleModels = new List<SampleModel>
                    {
                        new SampleModel
                        {
                            Id = "1",
                            SampleData = "SampleSampleData1",
                            Key = "SampleKey1"
                        },
                        new SampleModel
                        {
                            Id = "2",
                            SampleData = "SampleSampleData2",
                            Key = "SampleKey2"
                        }
                    };

                    var (cosmosClientMock, containerMock) = SetupCosmosServices(serviceProvider, sampleModels, sampleModel, uri);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICosmosService<Test>>();
                    var uutConcrete = (CosmosService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync<SampleModel>(sampleModels).ConfigureAwait(false);

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

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.Cosmos, insertTelemetryRequestObserved.TelemetryType);

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
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<CosmosService<Test>>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());

            //--Encryption

            //--Infrastructure
            serviceCollection.AddSingleton<ICosmosService<Test>, CosmosService<Test>>();
            serviceCollection.AddSingleton(Mock.Of<Container>());
            serviceCollection.AddSingleton(Mock.Of<CosmosClient>());
            serviceCollection.AddSingleton(Mock.Of<ICosmosFactory>());

            //--Options
            serviceCollection.AddOptions();
            var azureTableServiceOptions = new CosmosServiceOptions<Test>
            {
                ConnectionString = CONNECTION_STRING,
                ContainerId = "SampleContainerId",
                DatabaseId = "DatabaseId"
            };
            var options = Options.Create(azureTableServiceOptions);
            serviceCollection.AddSingleton<IOptions<CosmosServiceOptions<Test>>>(options);

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

        private (Mock<ILoggerService<CosmosService<Test>>>, List<Dictionary<string, object>>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<CosmosService<Test>>>();
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

        private (Mock<CosmosClient>, Mock<Container>) SetupCosmosServices(IServiceProvider serviceProvider, List<SampleModel> sampleModels, SampleModel sampleModel, Uri uri, bool throws = false, bool throwsNotFound = false, bool throwsPreconditionFailed = false)
        {
            var feedResponseMock = CreateFeedResponse(sampleModels);
            var feedIteratorMock = CreateFeedIterator(feedResponseMock.Object);
            var containerMock = CreateContainer(serviceProvider, feedIteratorMock.Object, sampleModel, throws, throwsNotFound, throwsPreconditionFailed);
            var cosmosClientMock = CreateCosmosClient(serviceProvider, containerMock.Object, uri);
            var cosmosFactoryMock = CreateCosmosFactory(serviceProvider, cosmosClientMock.Object, containerMock.Object);

            return (cosmosClientMock, containerMock);
        }
        private Mock<FeedResponse<SampleModel>> CreateFeedResponse(List<SampleModel> sampleModels)
        {
            var feedResponseMock = new Mock<FeedResponse<SampleModel>>();
            feedResponseMock.Setup
            (
                feedResponse => feedResponse.GetEnumerator()
            )
            .Returns
            (
                sampleModels.GetEnumerator()
            );

            return feedResponseMock;
        }
        private Mock<FeedIterator<SampleModel>> CreateFeedIterator(FeedResponse<SampleModel> feedResponse)
        {
            var feedIteratorMock = new Mock<FeedIterator<SampleModel>>();
            var feedIteratorCalls = 0;

            feedIteratorMock.Setup
            (
                feedIterator => feedIterator.HasMoreResults
            )
            .Returns(() =>
            {
                feedIteratorCalls++;
                return feedIteratorCalls <= 1;
            });
            feedIteratorMock.Setup
            (
                feedIterator => feedIterator.ReadNextAsync
                (
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(feedResponse);

            return feedIteratorMock;
        }
        private Mock<Container> CreateContainer(IServiceProvider serviceProvider, FeedIterator<SampleModel> feedIterator, SampleModel sampleModel, bool throws = false, bool throwsNotFound = false, bool throwsPreconditionFailed = false)
        {
            var responseMessage = new ResponseMessage();
            var containerMock = serviceProvider.GetMock<Container>();
            var itemResponseMock = new Mock<ItemResponse<SampleModel>>();
            itemResponseMock
            .Setup(itemResponse => itemResponse.Resource)
            .Returns(sampleModel);

            if (throws)
            {
                containerMock
                .Setup
                (
                    container => container.GetItemQueryIterator<SampleModel>
                    (
                        It.IsAny<QueryDefinition>(),
                        It.IsAny<string>(),
                        It.IsAny<QueryRequestOptions>()
                    )
                )
                .Throws(new Exception());

                containerMock
               .Setup
               (
                   container => container.ReadItemAsync<SampleModel>
                   (
                       It.IsAny<string>(),
                       It.IsAny<PartitionKey>(),
                       It.IsAny<ItemRequestOptions>(),
                       It.IsAny<CancellationToken>()
                   )
               )
               .Throws(new Exception());

                containerMock
               .Setup
               (
                   container => container.DeleteItemAsync<SampleModel>
                   (
                       It.IsAny<string>(),
                       It.IsAny<PartitionKey>(),
                       It.IsAny<ItemRequestOptions>(),
                       It.IsAny<CancellationToken>()
                   )
               )
               .Throws(new Exception());

                containerMock
                .Setup
                (
                    container => container.CreateItemAsync<SampleModel>
                    (
                        It.IsAny<SampleModel>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws(new Exception());

                containerMock
                .Setup
                (
                    container => container.UpsertItemAsync<SampleModel>
                    (
                        It.IsAny<SampleModel>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws(new Exception());

                containerMock
                .Setup
                (
                    container => container.UpsertItemStreamAsync
                    (
                        It.IsAny<Stream>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws(new Exception());

                containerMock
                .Setup
                (
                    container => container.CreateItemStreamAsync
                    (
                        It.IsAny<Stream>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws(new Exception());

                containerMock
                .Setup
                (
                    container => container.DeleteItemStreamAsync
                    (
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .Throws(new Exception());

                return containerMock;
            }

            if (throwsNotFound)
            {
                containerMock
               .Setup
               (
                   container => container.ReadItemAsync<SampleModel>
                   (
                       It.IsAny<string>(),
                       It.IsAny<PartitionKey>(),
                       It.IsAny<ItemRequestOptions>(),
                       It.IsAny<CancellationToken>()
                   )
               )
               .Throws(new CosmosException("", System.Net.HttpStatusCode.NotFound, 0, "", 0));

                return containerMock;
            }

            if (throwsPreconditionFailed)
            {
                containerMock
               .Setup
               (
                   container => container.UpsertItemAsync<SampleModel>
                   (
                       It.IsAny<SampleModel>(),
                       It.IsAny<PartitionKey>(),
                       It.IsAny<ItemRequestOptions>(),
                       It.IsAny<CancellationToken>()
                   )
               )
               .Throws(new CosmosException("", System.Net.HttpStatusCode.PreconditionFailed, 0, "", 0));

                return containerMock;
            }

            containerMock
            .Setup
            (
                container => container.GetItemQueryIterator<SampleModel>
                (
                    It.IsAny<QueryDefinition>(),
                    It.IsAny<string>(),
                    It.IsAny<QueryRequestOptions>()
                )
            )
            .Returns(feedIterator);

            containerMock
            .Setup
            (
                container => container.ReadItemAsync<SampleModel>
                (
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(itemResponseMock.Object);

            containerMock
            .Setup
            (
                container => container.DeleteItemAsync<SampleModel>
                (
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(itemResponseMock.Object);

            containerMock
            .Setup
            (
                container => container.CreateItemAsync<SampleModel>
                (
                    It.IsAny<SampleModel>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(itemResponseMock.Object);

            containerMock
            .Setup
            (
                container => container.UpsertItemAsync<SampleModel>
                (
                    It.IsAny<SampleModel>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(itemResponseMock.Object);


            containerMock
            .Setup
            (
                container => container.UpsertItemStreamAsync
                (
                    It.IsAny<Stream>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(responseMessage);

            containerMock
            .Setup
            (
                container => container.CreateItemStreamAsync
                (
                    It.IsAny<Stream>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(responseMessage);

            containerMock
            .Setup
            (
                container => container.DeleteItemStreamAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(responseMessage);
            

            return containerMock;
        }
        private Mock<CosmosClient> CreateCosmosClient(IServiceProvider serviceProvider, Container container, Uri uri)
        {
            var cosmosClientMock = serviceProvider.GetMock<CosmosClient>();

            cosmosClientMock
            .Setup
            (
                cosmosClient => cosmosClient.GetContainer
                (
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .Returns(container);

            cosmosClientMock
            .SetupGet
            (
                cosmosClient => cosmosClient.Endpoint
            )
            .Returns(uri);

            return cosmosClientMock;
        }
        private Mock<ICosmosFactory> CreateCosmosFactory(IServiceProvider serviceProvider, CosmosClient cosmosClient, Container container)
        {
            var cosmosFactoryMock = serviceProvider.GetMock<ICosmosFactory>();

            cosmosFactoryMock
            .Setup
            (
                cosmosFactory => cosmosFactory.CreateCosmosClient
                (
                    It.IsAny<CosmosServiceOptions>()
                )
            )
            .Returns(cosmosClient);

            cosmosFactoryMock
            .Setup
            (
                cosmosFactory => cosmosFactory.GetContainer
                (
                    It.IsAny<CosmosClient>(),
                    It.IsAny<CosmosServiceOptions>()
                )
            )
            .Returns(container);

            return cosmosFactoryMock;
        }
        #endregion

        public class Test : CosmosServiceOptionsType
        {
        }

        public class SampleModel : CosmosEntity
        {
            public string SampleData { get; set; }
        }
    }

}
