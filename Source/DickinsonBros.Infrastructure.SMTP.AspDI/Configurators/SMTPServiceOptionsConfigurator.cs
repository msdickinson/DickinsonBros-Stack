using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.SMTP.AspDI.Configurators
{
    public class SMTPServiceOptionsConfigurator<T, U> : IConfigureOptions<SMTPServiceOptions<T>>
    where T : SMTPServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public SMTPServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<SMTPServiceOptions<T>>.Configure(SMTPServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<U>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(SMTPServiceOptions<T>)}:{typeof(T).Name}";
            var cosmosServiceOptions = configuration.GetSection(path).Get<SMTPServiceOptions<T>>();
            configuration.Bind(path, options);

            options.Password = certificateEncryptionService.Decrypt(cosmosServiceOptions.Password);
        }
    }
}
