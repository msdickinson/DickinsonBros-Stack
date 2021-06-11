using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Application.Email.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IEmailIntegrationTests, EmailIntegrationTests>();
            return serviceCollection;
        }
    }
}
