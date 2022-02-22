using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace DickinsonBros.Encryption.JWT
{
    //TODO: This can be unit tested, 
    //https://dotnetcoretutorials.com/2020/11/18/generating-self-signed-certificates-for-unit-testing-in-c/
    //https://stackoverflow.com/questions/13806299/how-can-i-create-a-self-signed-certificate-using-c
    //Basicly it explains that You can shove in a cert into the code it self
    //Would require a Custom mock of ICertificateEncryptionService and have its FetchX509Certificate2 return a hard coded CERT

    [ExcludeFromCodeCoverage]
    public class JWTEncryptionService<T, U> : IJWTEncryptionService<T, U>
    where T : JWTEncryptionServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        internal const string TOKEN_TYPE = "Bearer";

        internal readonly string _issuer;
        internal readonly string _audience;

        internal readonly int _accessExpiresAfterMinutes;
        internal readonly int _refershExpiresAfterMinutes;

        internal readonly X509SecurityKey _x509SecurityKey;

        internal readonly IDateTimeService _dateTimeService;
        internal readonly ILoggerService<JWTEncryptionService<T, U>> _logger;
        internal readonly ICertificateEncryptionService<U> _certificateEncryptionService;

        public JWTEncryptionService
        (
            IOptions<JWTEncryptionServiceOptions<T, U>> jwtServiceOptions,
            IDateTimeService dateTimeService,
            ILoggerService<JWTEncryptionService<T, U>> logger,
            ICertificateEncryptionService<U> certificateEncryptionService
        )
        {
            _issuer = jwtServiceOptions.Value.Issuer;
            _audience = jwtServiceOptions.Value.Audience;
            _accessExpiresAfterMinutes = jwtServiceOptions.Value.AccessExpiresAfterMinutes;
            _refershExpiresAfterMinutes = jwtServiceOptions.Value.RefershExpiresAfterMinutes;
            _dateTimeService = dateTimeService;
            _certificateEncryptionService = certificateEncryptionService;
            _logger = logger;

            var x509Certificate = _certificateEncryptionService.FetchX509Certificate2();
            _x509SecurityKey = new X509SecurityKey(x509Certificate);
        }

        public GenerateTokensDescriptor GenerateTokens(string accessToken, string refershToken)
        {
            var accessTokenClaims = GetPrincipal(accessToken, false, TokenType.Access);
            var refreshTokenClaims = GetPrincipal(refershToken, true, TokenType.Refresh);


            if (accessTokenClaims == null ||
                 refreshTokenClaims == null ||
                 !accessTokenClaims.Identity.IsAuthenticated ||
                 !refreshTokenClaims.Identity.IsAuthenticated ||
                 accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value !=
                 refreshTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value)
            {
                return new GenerateTokensDescriptor()
                {
                    Authorized = false
                };
            }

            return GenerateTokens(accessTokenClaims.Claims);
        }

        public GenerateTokensDescriptor GenerateTokens(IEnumerable<Claim> claims)
        {
            var accessTokenExpiresInDateTime = _dateTimeService.GetDateTimeUTC().AddMinutes(_accessExpiresAfterMinutes);
            var refreshTokenExpiresInDateTime = _dateTimeService.GetDateTimeUTC().AddMinutes(_refershExpiresAfterMinutes);

            return new GenerateTokensDescriptor()
            {
                Tokens = new Tokens
                {
                    AccessToken = GenerateJWT(claims, accessTokenExpiresInDateTime, TokenType.Access),
                    AccessTokenExpiresIn = accessTokenExpiresInDateTime,
                    RefreshToken = GenerateJWT(claims, refreshTokenExpiresInDateTime, TokenType.Refresh),
                    RefreshTokenExpiresIn = refreshTokenExpiresInDateTime,
                    TokenType = TOKEN_TYPE
                },
                Authorized = true
            };
        }

        public string GenerateJWT(IEnumerable<Claim> claims, System.DateTime expiresDateTime, TokenType tokenType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                NotBefore = _dateTimeService.GetDateTimeUTC(),
                IssuedAt = _dateTimeService.GetDateTimeUTC(),
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(claims),
                Expires = expiresDateTime,
                SigningCredentials = new SigningCredentials(_x509SecurityKey, SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipal(string token, bool vaildateLifetime, TokenType tokenType)
        {
            var methodIdentifier = $"{nameof(JWTEncryptionService<T, U>)}.{nameof(GetPrincipal)}";
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = _x509SecurityKey,
                    ValidateLifetime = vaildateLifetime,
                    ValidAudience = _audience,
                    ValidIssuer = _issuer,
                };
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out SecurityToken securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogErrorRedacted(methodIdentifier, LogGroup.Infrastructure, ex);
                return null;

            }
        }
    }
}
