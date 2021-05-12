using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Configurators;
using DickinsonBros.Core.Redactor.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRedactorService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IRedactorService, RedactorService>();
            serviceCollection.AddSingleton<IConfigureOptions<RedactorServiceOptions>, RedactorServiceOptionsConfigurator>();
            return serviceCollection;
        }
    }
}
