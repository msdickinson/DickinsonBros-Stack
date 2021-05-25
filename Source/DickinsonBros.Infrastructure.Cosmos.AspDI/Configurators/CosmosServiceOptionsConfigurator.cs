using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.Cosmos.AspDI.Configurators
{
    public class CosmosServiceOptionsConfigurator<T, U> : IConfigureOptions<CosmosServiceOptions<T>>
    where T : CosmosServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CosmosServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<CosmosServiceOptions<T>>.Configure(CosmosServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<U>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(CosmosServiceOptions<T>)}:{typeof(T).Name}";
            var cosmosServiceOptions = configuration.GetSection(path).Get<CosmosServiceOptions<T>>();
            configuration.Bind(path, options);

            options.ConnectionString = certificateEncryptionService.Decrypt(cosmosServiceOptions.ConnectionString);
        }
    }
}
