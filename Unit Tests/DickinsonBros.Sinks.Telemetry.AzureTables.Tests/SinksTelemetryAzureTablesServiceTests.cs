using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Logger;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Tests
{
    [TestClass]
    public class SinksTelemetryAzureTablesServiceTests : BaseTest
    {
        private const string TABLE_NAME = "SAMPLE_TABLE_NAME";

        [TestMethod]
        public async Task NewTelemetryEvent_Runs_ItemInsertedIntoQueue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //Act
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Assert
                    Assert.AreEqual(1, uutConcrete._queueTelemetryDataEntitys.Count());

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UploadAsync_QueueIsEmpty_AzureTableServiceInsertBulkAsyncIsNotCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--IAzureTableService<SampleAzureTableServiceOptionsType>
                    var azureTableServiceMock = serviceProvider.GetMock<IAzureTableService<SampleAzureTableServiceOptionsType>>();
                    var itemsObserved = (IEnumerable<TelemetryDataEntity>)null;

                    azureTableServiceMock.Setup
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        )
                    )
                    .Callback
                    (
                        (IEnumerable<TelemetryDataEntity> items, string tableName, bool shouldSendTelemetry) =>
                        {
                            itemsObserved = items;
                        }
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //Act
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    await uutConcrete.UploadAsync().ConfigureAwait(false);
                    await uutConcrete.FlushAsync().ConfigureAwait(false);

                    //Assert
                    azureTableServiceMock.Verify
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        ),
                        Times.Once
                    );
                    
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UploadAsync_QueueIsNotEmpty_AzureTableServiceInsertBulkAsyncIsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--IAzureTableService<SampleAzureTableServiceOptionsType>
                    var azureTableServiceMock = serviceProvider.GetMock<IAzureTableService<SampleAzureTableServiceOptionsType>>();
                    var itemsObserved = (IEnumerable<TelemetryDataEntity>)null;

                    azureTableServiceMock.Setup
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        )
                    )
                    .Callback
                    (
                        (IEnumerable<TelemetryDataEntity> items, string tableName, bool shouldSendTelemetry) =>
                        {
                            itemsObserved = items;
                        }
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //-- SinksTelemetryAzureTablesService Setup
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Act
                    await uutConcrete.UploadAsync().ConfigureAwait(false);

                    //Assert
                    azureTableServiceMock.Verify
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            TABLE_NAME,
                            false
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(3, itemsObserved.Count());

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Uploader_QueueIsNotEmpty_AzureTableServiceInsertBulkAsyncIsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //--IAzureTableService<SampleAzureTableServiceOptionsType>
                    var azureTableServiceMock = serviceProvider.GetMock<IAzureTableService<SampleAzureTableServiceOptionsType>>();
                    azureTableServiceMock.Setup
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        )
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //-- SinksTelemetryAzureTablesService Setup
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Act
                    await Task.Delay(2000).ConfigureAwait(false);

                    //Assert
                    azureTableServiceMock.Verify
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        ),
                        Times.Once
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Uploader_AzureTableServiceInsertBulkAsyncThrows_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup


                    //-- IAzureTableService
                    var azureTableServiceMock = serviceProvider.GetMock<IAzureTableService<SampleAzureTableServiceOptionsType>>();
                    var exception = new Exception();
                    azureTableServiceMock.Setup
                    (
                        azureTableService => azureTableService.InsertBulkAsync
                        (
                            It.IsAny<IEnumerable<TelemetryDataEntity>>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>()
                        )
                    ).ThrowsAsync(exception);

                    //-- ILoggerService
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
                    loggerServiceMock.Setup
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, Exception, IDictionary<string, object>>
                    (
                        (message, LogGroup, exception, properties) =>
                        {
                            propertiesObserved = properties;
                        }
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //-- SinksTelemetryAzureTablesService Setup
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Act
                    await Task.Delay(2000).ConfigureAwait(false);
                    //await uut.FlushAsync().ConfigureAwait(false);

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"Unhandled exception in Uploader",
                            LogGroup.Infrastructure,
                            exception,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Flush_FlushCalledBeforeUploadIntervalEnds_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup


                    //-- loggerServiceMock
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
                    loggerServiceMock.Setup
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, IDictionary<string, object>>
                    (
                        (message, LogGroup, properties) =>
                        {
                            propertiesObserved = properties;
                        }
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //-- SinksTelemetryAzureTablesService Setup
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Act
                    await uut.FlushAsync().ConfigureAwait(false);

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"Uploader Task Canceled",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }
        
        [TestMethod]
        public async Task HostApplicationLifetime_HostApplicationLifetimeStoped_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- loggerServiceMock
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>>();
                    var propertiesObserved = (IDictionary<string, object>)null;
                    loggerServiceMock.Setup
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            It.IsAny<string>(),
                            It.IsAny<LogGroup>(),
                            It.IsAny<IDictionary<string, object>>()
                        )
                    )
                    .Callback<string, LogGroup, IDictionary<string, object>>
                    (
                        (message, LogGroup, properties) =>
                        {
                            propertiesObserved = properties;
                        }
                    );

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
                    var uutConcrete = (SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>)uut;

                    //-- SinksTelemetryAzureTablesService Setup
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());
                    uutConcrete.NewTelemetryEvent(new TelemetryItem());

                    //Act
                    uutConcrete._hostApplicationLifetime.StopApplication();

                    //Assert
                    loggerServiceMock.Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"Uploader Task Canceled",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        )
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>, SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>();
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<SinksTelemetryAzureTablesService<SampleAzureTableServiceOptionsType>>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());
            serviceCollection.AddSingleton(Mock.Of<IAzureTableService<SampleAzureTableServiceOptionsType>>());
            serviceCollection.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            serviceCollection.AddOptions();

            var sinksTelemetryAzureTablesServiceOptions = new SinksTelemetryAzureTablesServiceOptions
            {
                PartitionKey = "SamplePartitionKey",
                TableName = TABLE_NAME,
                UploadInterval = new TimeSpan(0, 0, 1)
            };
            var options = Options.Create(sinksTelemetryAzureTablesServiceOptions);
            serviceCollection.AddSingleton<IOptions<SinksTelemetryAzureTablesServiceOptions>>(options);
        
            return serviceCollection;
        }

    }

    public class SampleAzureTableServiceOptionsType : AzureTableServiceOptionsType { }
}
