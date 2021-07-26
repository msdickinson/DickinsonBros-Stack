using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Rest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRestIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IRestIntegrationTests, RestIntegrationTests>();
            return serviceCollection;
        }
    }
}
