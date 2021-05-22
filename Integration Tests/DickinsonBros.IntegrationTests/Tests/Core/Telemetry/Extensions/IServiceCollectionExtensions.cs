using DickinsonBros.IntegrationTests.Tests.Core.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Telemetry.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddTelemetryIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITelemetryIntegrationTests, TelemetryIntegrationTests>();
            return serviceCollection;
        }
    }
}
