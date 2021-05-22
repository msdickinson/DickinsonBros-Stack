using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Correlation.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICorrelationIntegrationTests, CorrelationIntegrationTests>();
            return serviceCollection;
        }
    }
}
