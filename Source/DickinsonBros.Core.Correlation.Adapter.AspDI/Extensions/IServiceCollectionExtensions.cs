using DickinsonBros.Core.Correlation.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICorrelationService, CorrelationService>();
            return serviceCollection;
        }
    }
}
