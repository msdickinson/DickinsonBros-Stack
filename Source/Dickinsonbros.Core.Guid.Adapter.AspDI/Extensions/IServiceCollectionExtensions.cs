using Dickinsonbros.Core.Guid.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddGuidService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IGuidService, GuidService>();
            return serviceCollection;
        }
    }
}
