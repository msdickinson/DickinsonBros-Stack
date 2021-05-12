using DickinsonBros.Core.Stopwatch.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddStopwatchService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IStopwatchService, StopwatchService>();
            return serviceCollection;
        }

        public static IServiceCollection AddStopwatchFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IStopwatchFactory, StopwatchFactory>();
            return serviceCollection;
        }
    }
}
