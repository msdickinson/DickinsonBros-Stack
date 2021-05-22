using DickinsonBros.IntegrationTests.Tests.Core.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Logger.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggerIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ILoggerIntegrationTests, LoggerIntegrationTests>();
            return serviceCollection;
        }
    }
}
