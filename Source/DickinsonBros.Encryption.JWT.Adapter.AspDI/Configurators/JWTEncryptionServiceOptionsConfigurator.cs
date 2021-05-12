using DickinsonBros.Encryption.JWT.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Encryption.JWT.Adapter.AspDI.Configurators
{
    public class JWTServiceOptionsConfigurator<T> : IConfigureOptions<JWTEncryptionServiceOptions<T>>
    where T: JWTEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public JWTServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<JWTEncryptionServiceOptions<T>>.Configure(JWTEncryptionServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(JWTEncryptionServiceOptions<T>)}:{typeof(T).Name}";
            var accountAPITestsOptions = configuration.GetSection(path).Get<JWTEncryptionServiceOptions<T>>();
            configuration.Bind(path, options);
        }
    }
}
