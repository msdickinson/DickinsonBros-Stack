using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Tests.Mocks;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL.Tests
{
    [TestClass]
    public class SQLServiceTests : BaseTest
    {
        #region ExecuteAsync

        [TestMethod]
        public async Task ExecuteAsync_Runs_DoesNotThrow()
        {
            //--Options
            var sqlServiceOptions = new SQLServiceOptions<SQLSampleType>
            {
                ConnectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;",
                ConnectionName = "SampleConnectionName"
            };

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    //-- Inputs

                    var sql = "select * from sampleTable where a=@A and b=@b";
                    var commandType = CommandType.Text;
                    var param = new { A = "a", B = "b" };

                    //-- Correlation
                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                        .SetupGet(correlationService => correlationService.CorrelationId);

                    //-- Guid
                    var guidServiceMock = serviceProvider.GetMock<IGuidService>();
                    guidServiceMock
                        .Setup(guidService => guidService.NewGuid());

                    //-- DateTime
                    var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
                    dateTimeServiceMock
                        .Setup(dateTimeService => dateTimeService.GetDateTimeUTC())
                        .Returns(new System.DateTime(2020, 1, 1));

                    //-- IStopwatchServiceMock
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- DbConnectionServiceMock
                    var idbConnectionServiceMock = serviceProvider.GetRequiredService<IDbConnectionService<SQLSampleType>>();
                    var dbConnectionServiceMock = (DbConnectionServiceMock<SQLSampleType>)idbConnectionServiceMock;

                    //-- UUT
                    var uut = serviceProvider.GetRequiredService<ISQLService<SQLSampleType>>();
                    var uutConcrete = (SQLService<SQLSampleType>)uut;

                    // Act
                    await uutConcrete.ExecuteAsync(sql, commandType, param).ConfigureAwait(false);

                    // Assert
                    Assert.IsTrue(dbConnectionServiceMock.Parameters.ContainsKey("A"));
                    Assert.AreEqual("a", dbConnectionServiceMock.Parameters["A"]);

                },
                serviceCollection => ConfigureServices(serviceCollection, sqlServiceOptions)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, SQLServiceOptions<SQLSampleType> sqlServiceOptions)
        {
            serviceCollection.AddSingleton<ISQLService<SQLSampleType>, SQLService<SQLSampleType>>();
            serviceCollection.AddSingleton<IDbConnectionService<SQLSampleType>, DbConnectionServiceMock<SQLSampleType>>();
            serviceCollection.AddSingleton<IDataTableService, DataTableService>();
            serviceCollection.AddMemoryCache();

            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<SQLService<SQLSampleType>>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());

            serviceCollection.AddOptions();

            var options = Options.Create(sqlServiceOptions);
            serviceCollection.AddSingleton((IOptions<SQLServiceOptions<SQLSampleType>>)options);
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

    public class SQLSampleType : SQLServiceOptionsType
    {
    }
}
