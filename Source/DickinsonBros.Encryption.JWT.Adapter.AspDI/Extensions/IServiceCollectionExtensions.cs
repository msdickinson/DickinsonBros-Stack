using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Configurators;
using DickinsonBros.Encryption.JWT.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddJWTEncryptionService<T, U>(this IServiceCollection serviceCollection)
        where T : JWTEncryptionServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton(typeof(IJWTEncryptionService<T, U>), typeof(JWTEncryptionService<T, U>));
            serviceCollection.TryAddSingleton(typeof(IConfigureOptions<JWTEncryptionServiceOptions<T, U>>), typeof(JWTServiceOptionsConfigurator<T, U>));
            return serviceCollection;
        }
    }
}
