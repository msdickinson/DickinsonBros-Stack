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
using DickinsonBros.Infrastructure.Cosmos.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

                    var queryDefinitionObserved = (QueryDefinition)propertiesObserved["queryDefinition"];
                    var queryRequestOptionsObserved = (QueryRequestOptions)propertiesObserved["queryRequestOptions"];
                    var itemsObserved = (List<SampleModel>)propertiesObserved["items"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

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

                    var queryDefinitionObserved = (QueryDefinition)propertiesObserved["queryDefinition"];
                    var queryRequestOptionsObserved = (QueryRequestOptions)propertiesObserved["queryRequestOptions"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var TelemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

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

        private (Mock<ILoggerService<CosmosService<Test>>>, IDictionary<string, object>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<CosmosService<Test>>>();
            var propertiesObserved = new Dictionary<string, object>();
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
                foreach(var prop in properties)
                {
                    propertiesObserved.Add(prop.Key, prop.Value);
                }
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
                foreach(var prop in properties)
                {
                    propertiesObserved.Add(prop.Key, prop.Value);
                }
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

        private (Mock<CosmosClient>, Mock<Container>) SetupCosmosServices(IServiceProvider serviceProvider, List<SampleModel> sampleModels, SampleModel sampleModel, Uri uri, bool throws = false)
        {
            var feedResponseMock = CreateFeedResponse(sampleModels);
            var feedIteratorMock = CreateFeedIterator(feedResponseMock.Object);
            var containerMock = CreateContainer(serviceProvider, feedIteratorMock.Object, sampleModel, throws);
            var cosmosClientMock = CreateCosmosClient(serviceProvider, containerMock.Object, uri);
            var cosmosFactoryMock =  CreateCosmosFactory(serviceProvider, cosmosClientMock.Object, containerMock.Object);

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
        private Mock<Container> CreateContainer(IServiceProvider serviceProvider, FeedIterator<SampleModel> feedIterator, SampleModel sampleModel, bool throws = false)
        {
            var containerMock = serviceProvider.GetMock<Container>();
            var itemResponseMock = new Mock<ItemResponse<SampleModel>>();
            itemResponseMock
            .Setup(itemResponse => itemResponse.Resource)
            .Returns(sampleModel);

            if (!throws)
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
                .Returns(feedIterator);
            }
            else
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
            }

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
