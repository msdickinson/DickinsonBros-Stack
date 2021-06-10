using DickinsonBros.Infrastructure.SMTP.Abstractions;
using MailKit.Net.Smtp;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SMTP
{
    [ExcludeFromCodeCoverage]
    public class SMTPClientFactory : ISMTPClientFactory
    {
        public ISmtpClient Create()
        {
            return new SmtpClient();
        }
    }
}
