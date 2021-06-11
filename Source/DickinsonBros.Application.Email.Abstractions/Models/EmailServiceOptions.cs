using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Application.Email.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class EmailServiceOptions
    {
        public bool SendSMTP { get; set; }
        public bool SaveToFile  { get; set; }
        public string SaveDirectory { get; set; }
    }
}
