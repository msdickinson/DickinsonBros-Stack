using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.AspDI.Configurators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.SQL.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSQLService<T, U>(this IServiceCollection serviceCollection)
        where T : SQLServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton<ISQLService<T>, SQLService<T>>();
            serviceCollection.TryAddSingleton<IConfigureOptions<SQLServiceOptions<T>>, SQLServiceOptionsConfigurator<T, U>>();
            serviceCollection.TryAddSingleton<IDataTableService, DataTableService>();
            return serviceCollection;
        }
    }
}
