using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.AspDI.Configurators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.Cosmos.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosService<T, U>(this IServiceCollection serviceCollection)
        where T : CosmosServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton<ICosmosService<T>, CosmosService<T>>();
            serviceCollection.TryAddSingleton<IConfigureOptions<CosmosServiceOptions<T>>, CosmosServiceOptionsConfigurator<T, U>>();
            serviceCollection.TryAddSingleton<ICosmosFactory, CosmosFactory>();
            return serviceCollection;
        }
    }
}
