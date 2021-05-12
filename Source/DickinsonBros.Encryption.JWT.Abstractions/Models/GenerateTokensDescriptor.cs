using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Encryption.JWT.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class GenerateTokensDescriptor
    {
        public bool Authorized { get; set; }
        public Tokens Tokens { get; set; }
    }
}
