using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;

namespace DickinsonBros.Application.Email.Abstractions.Models
{
    public class SendAsyncDescriptor
    {
        public bool SavedEmailToDisk { get; set; }
        public bool AttemptedSMTP { get; set; }
        public SendEmailDescriptor SendEmailDescriptor { get; set; }
    }
}
