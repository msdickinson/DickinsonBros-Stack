using MailKit.Net.Smtp;

namespace DickinsonBros.Infrastructure.SMTP.Abstractions
{
    public interface ISMTPClientFactory
    {
        public ISmtpClient Create();
    }
}