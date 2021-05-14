using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureTablesService<T>(this IServiceCollection serviceCollection)
        where T : AzureTableServiceOptionsType
        {
            serviceCollection.TryAddSingleton<IAzureTableService<T>, AzureTableService<T>>();
            return serviceCollection;
        }
    }
}
