using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.File.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFileIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IFileIntegrationTests, FileIntegrationTests>();
            return serviceCollection;
        }
    }
}
