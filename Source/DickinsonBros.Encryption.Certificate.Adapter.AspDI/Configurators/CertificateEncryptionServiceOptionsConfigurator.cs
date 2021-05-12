using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Encryption.Certificate.Adapter.AspDI.Configurators
{
    public class CertificateEncryptionServiceOptionsConfigurator<T> : IConfigureOptions<CertificateEncryptionServiceOptions<T>>
    where T : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CertificateEncryptionServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<CertificateEncryptionServiceOptions<T>>.Configure(CertificateEncryptionServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(CertificateEncryptionServiceOptions<T>)}:{typeof(T).Name}";
            var accountAPITestsOptions = configuration.GetSection(path).Get<CertificateEncryptionServiceOptions<T>>();
            configuration.Bind(path, options);
        }
    }
}
