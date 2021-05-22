using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.Certificate.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCertificateIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICertificateIntegrationTests, CertificateIntegrationTests>();
            return serviceCollection;
        }
    }
}
