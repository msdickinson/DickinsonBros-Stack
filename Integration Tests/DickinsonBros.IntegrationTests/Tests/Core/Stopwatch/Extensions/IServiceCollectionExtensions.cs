using DickinsonBros.IntegrationTests.Tests.Core.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.IntegrationTests.Tests.Core.Stopwatch.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddStopwatchIntegrationTests(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IStopwatchIntegrationTests, StopwatchIntegrationTests>();
            return serviceCollection;
        }
    }
}
