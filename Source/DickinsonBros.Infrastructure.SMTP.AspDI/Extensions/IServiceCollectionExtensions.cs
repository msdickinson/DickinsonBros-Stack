using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.AspDI.Configurators;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Infrastructure.SMTP.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSMTPService<T, U>(this IServiceCollection serviceCollection)
        where T : SMTPServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton<ISMTPService<T>, SMTPService<T>>();
            serviceCollection.TryAddSingleton<IConfigureOptions<SMTPServiceOptions<T>>, SMTPServiceOptionsConfigurator<T, U>>();
            serviceCollection.TryAddSingleton<ISMTPClientFactory, SMTPClientFactory>();
            return serviceCollection;
        }
    }
}
