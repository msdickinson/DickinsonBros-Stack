using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Encryption.JWT.Models
{
    [ExcludeFromCodeCoverage]
    public class JWTServiceOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessExpiresAfterMinutes { get; set; }
        public string AccessThumbPrint { get; set; }
        public string AccessStoreLocation { get; set; }
        public int RefershExpiresAfterMinutes { get; set; }
        public string RefershThumbPrint { get; set; }
        public string RefershStoreLocation { get; set; }
    }
}
