using DickinsonBros.Infrastructure.Rest;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Infrastructure.SMTP.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRestService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IRestService, RestService>();
            return serviceCollection;
        }
    }
}
