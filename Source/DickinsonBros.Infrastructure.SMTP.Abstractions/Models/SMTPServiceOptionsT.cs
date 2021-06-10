using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SMTP.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class SMTPServiceOptions<T> : SMTPServiceOptions
    where T : SMTPServiceOptionsType
    {

    }
}
