using DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.SQL.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSQLIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISQLIntegrationTests, SQLIntegrationTests>();
            return serviceCollection;
        }
    }
}
