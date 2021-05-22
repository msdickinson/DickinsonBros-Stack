using DickinsonBros.IntegrationTests.Tests.Core.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Redactor.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRedactorIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IRedactorIntegrationTests, RedactorIntegrationTests>();
            return serviceCollection;
        }
    }
}
