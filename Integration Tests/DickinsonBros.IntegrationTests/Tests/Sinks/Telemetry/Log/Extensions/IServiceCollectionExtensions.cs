using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.Log.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSinksTelemetryLogIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISinksTelemetryLogIntegrationTests, SinksTelemetryLogIntegrationTests>();
            return serviceCollection;
        }
    }
}
