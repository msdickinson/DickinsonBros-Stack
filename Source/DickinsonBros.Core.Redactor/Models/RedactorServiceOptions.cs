using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Redactor.Models
{
    [ExcludeFromCodeCoverage]
    public class RedactorServiceOptions
    {
        public string[] PropertiesToRedact { get; set; }
        public string[] RegexValuesToRedact { get; set; }
    }
}
