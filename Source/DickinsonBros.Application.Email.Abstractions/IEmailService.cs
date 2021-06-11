using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using MimeKit;
using System.Threading.Tasks;

namespace DickinsonBros.Application.Email.Abstractions
{
    public interface IEmailService<T>
    where T : SMTPServiceOptionsType
    {
        bool IsValidEmailFormat(string email);
        Task<SendAsyncDescriptor> SendAsync(MimeMessage message);

    }
}
