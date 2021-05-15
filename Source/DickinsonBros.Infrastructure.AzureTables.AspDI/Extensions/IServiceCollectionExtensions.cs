using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using DickinsonBros.Infrastructure.AzureTables.Models;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Configurators;
using DickinsonBros.Infrastructure.AzureTables.Factories;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureTablesService<T, U>(this IServiceCollection serviceCollection)
        where T : AzureTableServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton<IAzureTableService<T>, AzureTableService<T>>(); 
            serviceCollection.TryAddSingleton<IConfigureOptions<AzureTableServiceOptions<T>>, AzureTableServiceOptionsConfigurator<T, U>>();
            serviceCollection.TryAddSingleton<ICloudStorageAccountFactory, CloudStorageAccountFactory>();
            serviceCollection.TryAddSingleton<ICloudTableClientFactory, CloudTableClientFactory>();
            return serviceCollection;
        }
    }
}
