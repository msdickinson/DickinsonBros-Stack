using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.Log.Tests
{
    [TestClass]
    public class SinksTelemetryLogServiceTests : BaseTest
    {
        [TestMethod]
        public async Task TelemetryWriterService_NewTelemetryEvent_Runs_LogsTelemetryItem()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var insertTelemetryRequest = new Core.Telemetry.Abstractions.Models.TelemetryItem
                    {
                    };


                    //-- loggerServiceMock
                    var loggerServiceMock = serviceProvider.GetMock<ILoggerService<SinksTelemetryLogService>>();

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
                    var uut = serviceProvider.GetRequiredService<ISinksTelemetryLogService>();
                    var uutConcrete = (SinksTelemetryLogService)uut;

                    //Act
                    uutConcrete.TelemetryWriterService_NewTelemetryEvent(insertTelemetryRequest);

                    //Assert
                    loggerServiceMock.Verify
                   (
                       loggerService => loggerService.LogInformationRedacted
                       (
                           $"SinksTelemetryLogService.InsertTelemetryRequest",
                           LogGroup.Infrastructure,
                           It.IsAny<IDictionary<string, object>>()
                       )
                   );

                    Assert.AreEqual(propertiesObserved.Count, 1);
                    Assert.AreEqual(propertiesObserved["telemetryItem"], insertTelemetryRequest);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISinksTelemetryLogService, SinksTelemetryLogService>();
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<SinksTelemetryLogService>>());

            return serviceCollection;
        }

    }
}
