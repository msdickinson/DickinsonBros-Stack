using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Encryption.JWT.Adapter.AspDI.Configurators
{
    public class JWTServiceOptionsConfigurator<T, U> : IConfigureOptions<JWTEncryptionServiceOptions<T, U>>
    where T : JWTEncryptionServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public JWTServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        void IConfigureOptions<JWTEncryptionServiceOptions<T, U>>.Configure(JWTEncryptionServiceOptions<T, U> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(JWTEncryptionServiceOptions<T, U>)}:{typeof(T).Name}";
            var accountAPITestsOptions = configuration.GetSection(path).Get<JWTEncryptionServiceOptions<T, U>>();
            configuration.Bind(path, options);
        }
    }
}
