using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Configurators;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Telemetry.Adapter.AspDI.Tests.Configurators
{
    [TestClass]
    public class TelemetryWriterServiceOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_ConfigureCalled()
        {
            var telemetryWriterServiceOptions = new TelemetryWriterServiceOptions
            {
                ApplicationName = "SampleApplicationName"
            };
            var configurationRoot = BuildConfigurationRoot(telemetryWriterServiceOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<TelemetryWriterServiceOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);
                    Assert.AreEqual(telemetryWriterServiceOptions.ApplicationName, options.ApplicationName);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<TelemetryWriterServiceOptions>, TelemetryWriterServiceOptionsConfigurator>();

            return serviceCollection;
        }

        #endregion
    }
}
