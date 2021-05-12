using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.DateTime.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDateTimeIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDateTimeIntegrationTests, DateTimeIntegrationTests>();
            return serviceCollection;
        }
    }
}
