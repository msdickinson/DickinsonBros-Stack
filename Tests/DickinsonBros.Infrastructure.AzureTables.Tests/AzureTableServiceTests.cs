using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
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
                 
                    var cloudTableMock = SetupCloudServices(serviceProvider, uri, tableName, tableResult);

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

        // public async Task DeleteAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task DeleteAsync_Runs_StopwatchStart()
        // public async Task DeleteAsync_Runs_GetTableReferenceCalled()
        // public async Task DeleteAsync_Runs_CloudTableExecuteAsyncCalled()
        // public async Task DeleteAsync_Runs_StopWatchStop()
        // public async Task DeleteAsync_Throws_LogErrorRedactedCalled()
        // public async Task DeleteAsync_Throws_ThrowsException()
        // public async Task DeleteAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region DeleteBulkAsync

        // public async Task DeleteBulkAsync_Runs_GetDateTimeUTCalled()
        // public async Task DeleteBulkAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task DeleteBulkAsync_Runs_StopwatchStart()
        // public async Task DeleteBulkAsync_Runs_GetTableReferenceCalled()
        // public async Task DeleteBulkAsync_Runs_CloudTableExecuteBatchAsync()
        // public async Task DeleteBulkAsync_Runs_StopWatchStop()
        // public async Task DeleteBulkAsync_Throws_LogErrorRedactedCalled()
        // public async Task DeleteBulkAsync_Throws_ThrowsException()
        // public async Task DeleteBulkAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region FetchAsync

        // public async Task FetchAsync_Runs_GetDateTimeUTCalled()
        // public async Task FetchAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task FetchAsync_Runs_StopwatchStart()
        // public async Task FetchAsync_Runs_GetTableReferenceCalled()
        // public async Task FetchAsync_Runs_CloudTableExecuteAsyncCalled()
        // public async Task FetchAsync_Runs_StopWatchStop()
        // public async Task FetchAsync_Throws_LogErrorRedactedCalled()
        // public async Task FetchAsync_Throws_ThrowsException()
        // public async Task FetchAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region InsertAsync

        // public async Task InsertAsync_Runs_GetDateTimeUTCalled()
        // public async Task InsertAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task InsertAsync_Runs_StopwatchStart()
        // public async Task InsertAsync_Runs_GetTableReferenceCalled()
        // public async Task InsertAsync_Runs_CloudTableExecuteAsyncCalled()
        // public async Task InsertAsync_Runs_StopWatchStop()
        // public async Task InsertAsync_Throws_LogErrorRedactedCalled()
        // public async Task InsertAsync_Throws_ThrowsException()
        // public async Task InsertAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region InsertBulkAsync

        // public async Task InsertBulkAsync_Runs_GetDateTimeUTCalled()
        // public async Task InsertBulkAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task InsertBulkAsync_Runs_StopwatchStart()
        // public async Task InsertBulkAsync_Runs_GetTableReferenceCalled()
        // public async Task InsertBulkAsync_Runs_CloudTableExecuteBatchAsync()
        // public async Task InsertBulkAsync_Runs_StopWatchStop()
        // public async Task InsertBulkAsync_Throws_LogErrorRedactedCalled()
        // public async Task InsertBulkAsync_Throws_ThrowsException()
        // public async Task InsertBulkAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region QueryAsync

        // public async Task InsertAsync_Runs_GetDateTimeUTCalled()
        // public async Task InsertAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task InsertAsync_Runs_StopwatchStart()
        // public async Task InsertAsync_Runs_GetTableReferenceCalled()
        // public async Task InsertAsync_Runs_CloudTableExecuteQuerySegmentedAsyncCalled()
        // public async Task InsertAsync_Runs_StopWatchStop()
        // public async Task InsertAsync_Throws_LogErrorRedactedCalled()
        // public async Task InsertAsync_Throws_ThrowsException()
        // public async Task InsertAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region UpsertAsync

        // public async Task UpsertAsync_Runs_GetDateTimeUTCalled()
        // public async Task UpsertAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task UpsertAsync_Runs_StopwatchStart()
        // public async Task UpsertAsync_Runs_GetTableReferenceCalled()
        // public async Task UpsertAsync_Runs_CloudTableExecuteAsyncCalled()
        // public async Task UpsertAsync_Runs_StopWatchStop()
        // public async Task UpsertAsync_Throws_LogErrorRedactedCalled()
        // public async Task UpsertAsync_Throws_ThrowsException()
        // public async Task UpsertAsync_Runs_TelemetryServiceWriterInsertAsync()

        #endregion

        #region UpsertBulkAsync

        // public async Task UpsertBulkAsync_Runs_GetDateTimeUTCalled()
        // public async Task UpsertBulkAsync_Runs_StopwatchFactory.NewStopwatchService()
        // public async Task UpsertBulkAsync_Runs_StopwatchStart()
        // public async Task UpsertBulkAsync_Runs_GetTableReferenceCalled()
        // public async Task UpsertBulkAsync_Runs_CloudTableExecuteBatchAsync()
        // public async Task UpsertBulkAsync_Runs_StopWatchStop()
        // public async Task UpsertBulkAsync_Throws_LogErrorRedactedCalled()
        // public async Task UpsertBulkAsync_Throws_ThrowsException()
        // public async Task UpsertBulkAsync_Runs_TelemetryServiceWriterInsertAsync()

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

        private Mock<CloudTableClient> SetupCloudServices(IServiceProvider serviceProvider, Uri uri, string tableName, TableResult tableResult)
        {
            var cloudTableMock = CreateCloudTableMock(uri, tableResult);
            var cloudTableClientMock = CreateCloudTableClientMock(uri, tableName, cloudTableMock.Object);
            var cloudStorageAccountMock = CreateCloudStorageAccount(serviceProvider, uri);
            var cloudClientFactoryMock = CreateCloudClientFactoryMock(serviceProvider, cloudStorageAccountMock, cloudTableClientMock.Object);
            var cloudStorageAccountFactoryMock = CreateCloudStorageAccountFactoryMock(serviceProvider, cloudStorageAccountMock);

            return cloudTableClientMock;
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
