using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Guid.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddGuidIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IGuidIntegrationTests, GuidIntegrationTests>();
            return serviceCollection;
        }
    }
}
