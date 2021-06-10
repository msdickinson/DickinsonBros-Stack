using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.DNS.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDNSIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDNSIntegrationTests, DNSIntegrationTests>();
            return serviceCollection;
        }
    }
}
