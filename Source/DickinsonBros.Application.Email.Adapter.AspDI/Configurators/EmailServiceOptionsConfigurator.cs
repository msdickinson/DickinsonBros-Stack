using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Application.Email.Adapter.AspDI.Configurators
{
    public class EmailServiceOptionsConfigurator<T> : IConfigureOptions<EmailServiceOptions<T>>
    where T : SMTPServiceOptionsType
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public EmailServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<EmailServiceOptions<T>>.Configure(EmailServiceOptions<T> options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(EmailServiceOptions<T>)}:{typeof(T).Name}";
            configuration.Bind(path, options);
        }
    }
}
