using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using MimeKit;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SMTP.Abstractions
{
    public interface ISMTPService<T>
    where T : SMTPServiceOptionsType
    {
        public Task<SendEmailDescriptor> SendEmailAsync(MimeMessage mimeMessage);
    }
}