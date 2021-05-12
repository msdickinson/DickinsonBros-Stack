using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Encryption.JWT.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class Tokens
    {
        public string AccessToken { get; set; }
        public System.DateTime AccessTokenExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public System.DateTime RefreshTokenExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
