using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.SMTP.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSMTPIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISMTPIntegrationTests, SMTPIntegrationTests>();
            return serviceCollection;
        }
    }
}
