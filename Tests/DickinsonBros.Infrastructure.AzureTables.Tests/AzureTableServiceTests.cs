using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Factories;
using DickinsonBros.Infrastructure.AzureTables.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Tests
{
    [TestClass]
    public class AzureTableServiceTests : BaseTest
    {
        private const string CONNECTION_STRING = "SampleConnectionString";

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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        
        [TestMethod]
        public async Task DeleteAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_CloudTableExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteAsync
                        (
                            It.IsAny<TableOperation>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_StopwatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_ReturnsTableResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableResult.Etag, observed.Etag);
                    Assert.AreEqual(tableResult.HttpStatusCode, observed.HttpStatusCode);
                    Assert.AreEqual(sampleEntityResult, observed.Result);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.DeleteAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Delete {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
      
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteAsync_WhenNotExceptional_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    var observed = await uutConcrete.DeleteAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.DeleteAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Delete {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_CloudTableExecuteBatchAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteBatchAsync
                        (
                            It.IsAny<TableBatchOperation>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_StopWatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.DeleteBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"DeleteBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_ReturnsTableBatchResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableBatchResult, observed.ToList().First());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task DeleteBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.DeleteBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.DeleteBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"DeleteBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_CloudTableExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteAsync
                        (
                            It.IsAny<TableOperation>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_StopwatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_ReturnsTableResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableResult.Etag, observed.Etag);
                    Assert.AreEqual(tableResult.HttpStatusCode, observed.HttpStatusCode);
                    Assert.AreEqual(sampleEntityResult, observed.Result);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.InsertAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Insert {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_ThrowsConflictException_TelemetryResponseStateSetToConflict()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult, "Conflict");

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.InsertAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Insert {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Conflict", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Conflict, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertAsync_WhenNotExceptional_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    var observed = await uutConcrete.InsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.InsertAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Insert {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_CloudTableExecuteBatchAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteBatchAsync
                        (
                            It.IsAny<TableBatchOperation>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_StopWatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.InsertBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"InsertBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Runs_ReturnsTableBatchResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableBatchResult, observed.ToList().First());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_ShouldSendTelemetryIsTrue_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_ShouldSendTelemetryIsFalse_TelemetryServiceWriterInsertAsyncNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.InsertBulkAsync(sampleEntitys, tableName, false).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Never
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task InsertBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.InsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.InsertBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"InsertBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_CloudTableExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteAsync
                        (
                            It.IsAny<TableOperation>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_StopwatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_ReturnsTableResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableResult.Etag, observed.Etag);
                    Assert.AreEqual(tableResult.HttpStatusCode, observed.HttpStatusCode);
                    Assert.AreEqual(sampleEntityResult, observed.Result);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.FetchAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Fetch {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_Runs_TelemetryServiceWriterFetchAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task FetchAsync_WhenNotExceptional_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    string partitionKey = "SamplePartitionKey";
                    string rowkey = "SampleRowKey";
                    string tableName = "SampleTableName";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    var observed = await uutConcrete.FetchAsync<SampleEntity>(partitionKey, rowkey, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.FetchAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Fetch {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

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
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task QueryAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task QueryAsync_Runs_GetTableRefernceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task QueryAsync_Runs_ExecuteQuerySegmentedAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteQuerySegmentedAsync<SampleEntity>
                        (
                            tableQuery, 
                            null
                        ),
                        Times.Exactly(1)
                    );

                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteQuerySegmentedAsync<SampleEntity>
                        (
                            tableQuery,
                            createTableQuerySegmentMock.ContinuationToken
                        ),
                        Times.Exactly(1)
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        
        [TestMethod]
        public async Task QueryAsync_Runs_StopwatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task QueryAsync_WhenNotExceptional_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.QueryAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Query {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);
                    
                    var resultsObserved = (List<SampleEntity>)propertiesObserved["results"];
                    Assert.IsNotNull(resultsObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task QueryAsync_Runs_ReturnsTableBatchResults()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(0, observed.Count());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        exceptionObserved = exception;
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.QueryAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Query {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        exceptionObserved = exception;
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var tableQuery = new TableQuery<SampleEntity>();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var createTableQuerySegmentMock = CreateTableQuerySegmentMock(true);
                    var createTableQuerySegmentNullTokenMock = CreateTableQuerySegmentMock(false);
                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableQuery, createTableQuerySegmentMock, createTableQuerySegmentNullTokenMock);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    var observed = await uutConcrete.QueryAsync(tableName, tableQuery).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        // public async Task InsertAsync_Runs_TelemetryServiceWriterInsertAsync()

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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_CloudTableExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteAsync
                        (
                            It.IsAny<TableOperation>()
                        ),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_StopwatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_ReturnsTableResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableResult.Etag, observed.Etag);
                    Assert.AreEqual(tableResult.HttpStatusCode, observed.HttpStatusCode);
                    Assert.AreEqual(sampleEntityResult, observed.Result);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.UpsertAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Upsert {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertAsync_WhenNotExceptional_LogInfoRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act

                    var observed = await uutConcrete.UpsertAsync(sampleEntity, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.UpsertAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"Upsert {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Status Code: 200", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
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
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopWatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_StopwatchStart()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_GetTableReferenceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableClientMock
                    .Verify
                    (
                        cloudTableClient => cloudTableClient.GetTableReference(tableName),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_CloudTableExecuteBatchAsync()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    cloudTableMock
                    .Verify
                    (
                        cloudTable => cloudTable.ExecuteBatchAsync
                        (
                            It.IsAny<TableBatchOperation>()
                        ),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_StopWatchStop()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once
                    );

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                        propertiesObserved = properties;
                    });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.UpsertBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"UpsertBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.Successful, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_ReturnsTableBatchResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    Assert.AreEqual(tableBatchResult, observed.ToList().First());

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Runs_TelemetryServiceWriterInsertAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServices(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    var observed = await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("http://services.odata.org/", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual("Successful", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Throws_ThrowsException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();
                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)null;
                    telemetryServiceWriterMock
                    .Setup
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryRequest>()
                        )
                    )
                    .Callback((InsertTelemetryRequest insertTelemetryRequest) =>
                    {
                        insertTelemetryRequestObserved = insertTelemetryRequest;
                    });

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task UpsertBulkAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Input
                    var sampleEntity = new SampleEntity
                    {
                        Data = "SampleData",
                        ETag = "SampleETag",
                        PartitionKey = "SamplePartitionKey",
                        RowKey = "SampleRowKey"
                    };

                    var sampleEntitys = new List<SampleEntity> { sampleEntity };

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- Cloud Services 
                    var tableName = "SampleTableName";
                    var uri = new Uri("http://services.odata.org");

                    var sampleEntityResult = new SampleEntity
                    {
                        Data = sampleEntity.Data,
                        ETag = sampleEntity.ETag,
                        PartitionKey = sampleEntity.PartitionKey,
                        RowKey = sampleEntity.RowKey
                    };

                    var tableResult = new TableResult();
                    tableResult.Etag = sampleEntityResult.ETag;
                    tableResult.HttpStatusCode = 200;
                    tableResult.Result = sampleEntityResult;

                    var tableBatchResult = new TableBatchResult();
                    tableBatchResult.Add(tableResult);

                    var (cloudTableClientMock, cloudTableMock) = SetupCloudClientServicesToThrow(serviceProvider, uri, tableName, tableBatchResult);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<Test>>();
                    var exceptionObserved = (Exception)null;
                    var propertiesObserved = (IDictionary<string, object>)null;
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
                       exceptionObserved = exception;
                       propertiesObserved = properties;
                   });

                    //-- ITelemetryServiceWriter
                    var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryServiceWriter>();

                    //--uut
                    var uut = serviceProvider.GetRequiredService<IAzureTableService<Test>>();
                    var uutConcrete = (AzureTableService<Test>)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.UpsertBulkAsync(sampleEntitys, tableName).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IAzureTableService<Test>)}<{typeof(Test).Name}>.{nameof(IAzureTableService<Test>.UpsertBulkAsync)}<{typeof(SampleEntity).Name}>",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var insertTelemetryRequestObserved = (InsertTelemetryRequest)propertiesObserved["insertTelemetryRequest"];

                    Assert.AreEqual(uri.ToString(), insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual($"UpsertBulk {typeof(SampleEntity).Name}. Tablename: {tableName}", insertTelemetryRequestObserved.SignalRequest);
                    Assert.AreEqual(TelemetryType.AzureTable, insertTelemetryRequestObserved.TelemetryType);
                    Assert.AreEqual("Exceptional", insertTelemetryRequestObserved.SignalResponse);
                    Assert.AreEqual(TelemetryResponseState.UnHandledException, insertTelemetryRequestObserved.TelemetryResponseState);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region UpdateTelemetryRequest



        #endregion

        #region Helpers

        private Mock<IDateTimeService> CreateDateTimeServiceMock(IServiceProvider serviceProvider, DateTime getDateTimeUTCResponse)
        {
            var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
            dateTimeServiceMock.Setup(stopwatchService => stopwatchService.GetDateTimeUTC()).Returns(getDateTimeUTCResponse);

            return dateTimeServiceMock;
        }

        private Mock<IStopwatchService> CreateStopWatchServiceMock(IServiceProvider serviceProvider)
        {
            //-- IStopwatchService
            var stopwatchServiceMock = serviceProvider.GetMock<IStopwatchService>();
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Start());
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Stop());
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.ElapsedMilliseconds).Returns(0);

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
        private Mock<TableQuerySegment<SampleEntity>> CreateTableQuerySegmentMockx(bool nullTableContinuationToken)
        {
            var tableQuerySegmentMock = new Mock<TableQuerySegment<SampleEntity>>();
            var tableContinuationToken = new TableContinuationToken();

            if(nullTableContinuationToken)
            {
                tableContinuationToken = (TableContinuationToken)null;
            }

            tableQuerySegmentMock.SetupGet
            (
                cloudTable => cloudTable.ContinuationToken
            )
            .Returns
            (
                tableContinuationToken
            );
           

            return tableQuerySegmentMock;
        }

        private TableQuerySegment<SampleEntity> CreateTableQuerySegmentMock(bool withTableContinuationToken)
        {            
            var ctor = typeof(TableQuerySegment<SampleEntity>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);
            MethodInfo setTokenMethod = typeof(TableQuerySegment<SampleEntity>).GetMethod("set_ContinuationToken", BindingFlags.NonPublic | BindingFlags.Instance);
            var mockQuerySegment = ctor.Invoke(new object[] { new List<SampleEntity>() }) as TableQuerySegment<SampleEntity>;

            if(withTableContinuationToken)
            {
                var continuationToken = new TableContinuationToken();
                setTokenMethod.Invoke(mockQuerySegment, new object[] { continuationToken });
            }

            return mockQuerySegment;
        }

        private TableQuerySegment<SampleEntity> CreateTableQuerySegmentMock()
        {
            var tableQuerySegmentMock = new Mock<TableQuerySegment<SampleEntity>>();
            var continuationToken = new TableContinuationToken();

            var ctor = typeof(TableQuerySegment<SampleEntity>)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(c => c.GetParameters().Count() == 1);
            MethodInfo setTokenMethod = typeof(TableQuerySegment<SampleEntity>).GetMethod("set_ContinuationToken", BindingFlags.NonPublic | BindingFlags.Instance);

            var mockQuerySegment = ctor.Invoke(new object[] { new List<SampleEntity>() }) as TableQuerySegment<SampleEntity>;
            var mockQuerySegmentNotNull = ctor.Invoke(new object[] { new List<SampleEntity>() }) as TableQuerySegment<SampleEntity>;

            return mockQuerySegment;
        }

        private Mock<CloudTable> CreateCloudTableMock(Uri uri, TableQuery<SampleEntity> tableQuery, TableQuerySegment<SampleEntity> tableQuerySegment, TableQuerySegment<SampleEntity> tableQuerySegmentWithNullToken)
        {
            var tableClientConfiguration = new TableClientConfiguration();
            var mockCloudTableMock = new Mock<CloudTable>(uri, tableClientConfiguration);
            mockCloudTableMock.SetupSequence
            (
                cloudTable => cloudTable.ExecuteQuerySegmentedAsync
                (
                    tableQuery,
                    It.IsAny<TableContinuationToken>()
                )
            )
            .ReturnsAsync
            (
               () => { return tableQuerySegment;  }
            )
            .ReturnsAsync
            (
               () => { return tableQuerySegmentWithNullToken; }
            );

            return mockCloudTableMock;
        }

        private Mock<CloudTable> CreateCloudTableMockToThrow(Uri uri, TableQuery<SampleEntity> tableQuery, TableQuerySegment<SampleEntity> tableQuerySegment, TableQuerySegment<SampleEntity> tableQuerySegmentWithNullToken)
        {
            var tableClientConfiguration = new TableClientConfiguration();
            var mockCloudTableMock = new Mock<CloudTable>(uri, tableClientConfiguration);
            mockCloudTableMock.SetupSequence
            (
                cloudTable => cloudTable.ExecuteQuerySegmentedAsync
                (
                    tableQuery,
                    It.IsAny<TableContinuationToken>()
                )
            )
            .Throws(new Exception());

            return mockCloudTableMock;
        }


        

        private Mock<CloudTable> CreateCloudTableMock(Uri uri, TableResult executeAsyncResult)
        {
            var tableOperationObserved = (TableOperation)null;
            var tableClientConfiguration = new TableClientConfiguration();
            var mockCloudTableMock = new Mock<CloudTable>(uri, tableClientConfiguration);

            mockCloudTableMock.Setup
            (
                cloudTable => cloudTable.ExecuteAsync
                (
                    It.IsAny<TableOperation>()
                )
            )
            .Callback((TableOperation tableOperation) =>
            {
                tableOperationObserved = tableOperation;
            })
            .ReturnsAsync
            (
                executeAsyncResult
            );

            return mockCloudTableMock;
        }

        private Mock<CloudTable> CreateCloudTableMock(Uri uri, TableBatchResult tableBatchResult)
        {
            var tableClientConfiguration = new TableClientConfiguration();
            var mockCloudTableMock = new Mock<CloudTable>(uri, tableClientConfiguration);

            mockCloudTableMock.Setup
            (
                cloudTable => cloudTable.ExecuteBatchAsync
                (
                    It.IsAny<TableBatchOperation>()
                )
            )
            .ReturnsAsync
            (
                tableBatchResult
            );

            return mockCloudTableMock;
        }

        private Mock<CloudTableClient> CreateCloudTableClientMock(Uri uri, string tableName, CloudTable getTableReferenceResult)
        {
            var storageUri = new StorageUri(uri);
            var storageCredentials = new StorageCredentials();
            var cloudTableClientMock = new Mock<CloudTableClient>(storageUri, storageCredentials);

            cloudTableClientMock
            .Setup
            (
                cloudTableClient => cloudTableClient.GetTableReference
                (
                    tableName
                )
            )
            .Returns
            (
                getTableReferenceResult
            );

            return cloudTableClientMock;
        }
        private Mock<CloudTableClient> CreateCloudTableClientMockToThrow(Uri uri, string tableName, CloudTable getTableReferenceResult, string exceptionMessage)
        {
            var storageUri = new StorageUri(uri);
            var storageCredentials = new StorageCredentials();
            var cloudTableClientMock = new Mock<CloudTableClient>(storageUri, storageCredentials);

            cloudTableClientMock
            .Setup
            (
                cloudTableClient => cloudTableClient.GetTableReference
                (
                    tableName
                )
            )
            .Throws(new Exception(exceptionMessage));

            return cloudTableClientMock;
        }

        private CloudStorageAccount CreateCloudStorageAccount(IServiceProvider serviceProvider, Uri uri)
        {
            return new CloudStorageAccount(new StorageCredentials("SampleName"), uri);
        }

        private Mock<ICloudStorageAccountFactory> CreateCloudStorageAccountFactoryMock(IServiceProvider serviceProvider, CloudStorageAccount cloudStorageAccount)
        {
            var cloudStorageAccountFactoryMock = serviceProvider.GetMock<ICloudStorageAccountFactory>();

            cloudStorageAccountFactoryMock
            .Setup
            (
                cloudStorageAccountFactory => cloudStorageAccountFactory.CreateCloudStorageAccount
                (
                    CONNECTION_STRING
                )
            )
            .Returns
            (
                cloudStorageAccount
            );

            return cloudStorageAccountFactoryMock;
        }
     
        private Mock<ICloudTableClientFactory> CreateCloudClientFactoryMock(IServiceProvider serviceProvider, CloudStorageAccount cloudStorageAccount, CloudTableClient cloudTableClient)
        {
            var cloudTableClientFactoryMock = serviceProvider.GetMock<ICloudTableClientFactory>();

            cloudTableClientFactoryMock
            .Setup
            (
                cloudTableClientFactory => cloudTableClientFactory.CreateCloudTableClient
                (
                    cloudStorageAccount
                )
            )
            .Returns
            (
                cloudTableClient
            );
            return cloudTableClientFactoryMock;
        }

        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServices(IServiceProvider serviceProvider, Uri uri, string tableName, TableQuery<SampleEntity> tableQuery, TableQuerySegment<SampleEntity> tableQuerySegment, TableQuerySegment<SampleEntity> tableQuerySegmentWithNullToken)
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableQuery, tableQuerySegment, tableQuerySegmentWithNullToken);
            var cloudTableClientMock = CreateCloudTableClientMock(uri, tableName, cloudTableMock.Object);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }

 
        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServices(IServiceProvider serviceProvider, Uri uri, string tableName, TableBatchResult tableBatchResult)
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableBatchResult);
            var cloudTableClientMock = CreateCloudTableClientMock(uri, tableName, cloudTableMock.Object);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }

        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServices(IServiceProvider serviceProvider, Uri uri, string tableName, TableResult tableResult)
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableResult);
            var cloudTableClientMock = CreateCloudTableClientMock(uri, tableName, cloudTableMock.Object);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }

        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServicesToThrow(IServiceProvider serviceProvider, Uri uri, string tableName, TableQuery<SampleEntity> tableQuery, TableQuerySegment<SampleEntity> tableQuerySegment, TableQuerySegment<SampleEntity> tableQuerySegmentWithNullToken)
        {
            var cloudTableMock = CreateCloudTableMockToThrow(uri, tableQuery, tableQuerySegment, tableQuerySegmentWithNullToken);
            var cloudTableClientMock = CreateCloudTableClientMock(uri, tableName, cloudTableMock.Object);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }
        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServicesToThrow(IServiceProvider serviceProvider, Uri uri, string tableName, TableResult tableResult, string exceptionMessage = "")
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableResult);
            var cloudTableClientMock = CreateCloudTableClientMockToThrow(uri, tableName, cloudTableMock.Object, exceptionMessage);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }
        private (Mock<CloudTableClient>, Mock<CloudTable>) SetupCloudClientServicesToThrow(IServiceProvider serviceProvider, Uri uri, string tableName, TableBatchResult tableBatchResult, string exceptionMessage = "")
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableBatchResult);
            var cloudTableClientMock = CreateCloudTableClientMockToThrow(uri, tableName, cloudTableMock.Object, exceptionMessage);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return (cloudTableClientMock, cloudTableMock);
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAzureTableService<Test>, AzureTableService<Test>>();
            serviceCollection.AddSingleton(Mock.Of<ICloudStorageAccountFactory>());
            serviceCollection.AddSingleton(Mock.Of<ICloudTableClientFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<Test>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryServiceWriter>());
            
            serviceCollection.AddOptions();

            var azureTableServiceOptions = new AzureTableServiceOptions<Test>
            {
                ConnectionString = CONNECTION_STRING
            };
            var options = Options.Create(azureTableServiceOptions);
            serviceCollection.AddSingleton<IOptions<AzureTableServiceOptions<Test>>>(options);

            //???
            var configurationRoot = BuildConfigurationRoot(azureTableServiceOptions);
            serviceCollection.AddSingleton<IConfiguration>(configurationRoot);

            return serviceCollection;
        }

        public class Test : AzureTableServiceOptionsType
        {
        }
        public class SampleEntity : TableEntity
        {
            public string Data { get; set; }
        }

        #endregion

    }
}
