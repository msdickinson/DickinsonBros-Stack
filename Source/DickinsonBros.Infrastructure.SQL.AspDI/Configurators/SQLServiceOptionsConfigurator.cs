using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.Cosmos.AspDI.Configurators
{
    public class SQLServiceOptionsConfigurator<T, U> : IConfigureOptions<SQLServiceOptions<T>>
    where T : SQLServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public SQLServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<SQLServiceOptions<T>>.Configure(SQLServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<U>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(SQLServiceOptions<T>)}:{typeof(T).Name}";
            var cosmosServiceOptions = configuration.GetSection(path).Get<SQLServiceOptions<T>>();
            configuration.Bind(path, options);

            options.ConnectionString = certificateEncryptionService.Decrypt(cosmosServiceOptions.ConnectionString);
        }
    }
}
