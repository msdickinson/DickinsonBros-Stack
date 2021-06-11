using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Application.Email.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class EmailServiceOptions<T> : EmailServiceOptions
    where T : SMTPServiceOptionsType
    {

    }
}
