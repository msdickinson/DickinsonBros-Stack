using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Encryption.JWT.Models
{
    [ExcludeFromCodeCoverage]
    public class JWTEncryptionServiceOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessExpiresAfterMinutes { get; set; }
        public int RefershExpiresAfterMinutes { get; set; }
    }
}
