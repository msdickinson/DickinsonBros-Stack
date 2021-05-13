using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.JWT.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddJWTIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IJWTIntegrationTests, JWTIntegrationTests>();
            return serviceCollection;
        }
    }
}
