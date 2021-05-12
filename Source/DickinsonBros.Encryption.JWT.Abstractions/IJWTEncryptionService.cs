using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace DickinsonBros.Encryption.JWT.Abstractions
{
    public interface IJWTEncryptionService<T, U>
    where T : JWTEncryptionServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        string GenerateJWT(IEnumerable<Claim> claims, System.DateTime expiresDateTime, TokenType tokenType);
        GenerateTokensDescriptor GenerateTokens(IEnumerable<Claim> claims);
        GenerateTokensDescriptor GenerateTokens(string accessToken, string refershToken);
        ClaimsPrincipal GetPrincipal(string token, bool vaildateLifetime, TokenType tokenType);
    }
}
