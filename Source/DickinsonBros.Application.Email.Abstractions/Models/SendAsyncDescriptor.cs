using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Application.Email.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class SendAsyncDescriptor
    {
        public bool SavedEmailToDisk { get; set; }
        public bool AttemptedSMTP { get; set; }
        public SendEmailDescriptor SendEmailDescriptor { get; set; }
    }
}
