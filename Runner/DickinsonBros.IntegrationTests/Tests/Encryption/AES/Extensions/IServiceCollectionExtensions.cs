using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.AES.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAESIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IAESIntegrationTests, AESIntegrationTests>();
            return serviceCollection;
        }
    }
}
