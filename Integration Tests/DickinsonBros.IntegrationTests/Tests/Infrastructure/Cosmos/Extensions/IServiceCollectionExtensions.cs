using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICosmosIntegrationTests, CosmosIntegrationTests>();
            return serviceCollection;
        }
    }
}
