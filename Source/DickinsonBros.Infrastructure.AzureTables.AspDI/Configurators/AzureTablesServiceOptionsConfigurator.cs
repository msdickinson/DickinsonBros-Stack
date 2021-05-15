using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.AzureTables.AspDI.Configurators
{
    public class AzureTableServiceOptionsConfigurator<T, U> : IConfigureOptions<AzureTableServiceOptions<T>>
    where T : AzureTableServiceOptionsType 
    where U : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public AzureTableServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<AzureTableServiceOptions<T>>.Configure(AzureTableServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<U>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(AzureTableServiceOptions<T>)}:{typeof(T).Name}";
            var azureTableServiceOptions = configuration.GetSection(path).Get<AzureTableServiceOptions<T>>();

            configuration.Bind(path, options);

            options.ConnectionString = certificateEncryptionService.Decrypt(azureTableServiceOptions.ConnectionString);
        }
    }
}
