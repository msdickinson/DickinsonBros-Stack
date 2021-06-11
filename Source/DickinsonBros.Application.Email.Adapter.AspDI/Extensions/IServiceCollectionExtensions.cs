using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Application.Email.Adapter.AspDI.Configurators;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Application.Email.Adapter.AspDI.Extensions
{ 
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService<T>(this IServiceCollection serviceCollection)
        where T : SMTPServiceOptionsType
        {
            serviceCollection.TryAddSingleton<IEmailService<T>, EmailService<T>>();
            serviceCollection.AddSingleton<IConfigureOptions<EmailServiceOptions<T>>, EmailServiceOptionsConfigurator<T>>();
            return serviceCollection;
        }
    }
}
