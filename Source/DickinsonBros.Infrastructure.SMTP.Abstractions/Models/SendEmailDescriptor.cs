using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SMTP.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class SendEmailDescriptor
    { 
        public SendEmailResult SendEmailResult { get; set; }
        public Exception Exception { get; set; }
    }
}
