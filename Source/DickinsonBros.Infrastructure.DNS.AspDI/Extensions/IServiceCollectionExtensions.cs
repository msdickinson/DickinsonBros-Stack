using DickinsonBros.Infrastructure.DNS.Abstractions;
using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Infrastructure.DNS.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDNSService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDNSService, DNSService>();
            serviceCollection.TryAddSingleton<ILookupClient, LookupClient>();

            return serviceCollection;
        }
    }
}
