using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureTablesIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IAzureTablesIntegrationTests, AzureTablesIntegrationTests>();
            return serviceCollection;
        }
    }
}
