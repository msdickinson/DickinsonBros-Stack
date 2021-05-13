using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.JWT
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "JWT", Group = "Encryption")]
    public class JWTIntegrationTests : IJWTIntegrationTests
    {
        public IJWTEncryptionService<RunnerJWTEncryptionServiceOptionsType, Configuration> _jwtTEncryptionService;
        public IDateTimeService _dateTimeService;

        public JWTIntegrationTests
        (
            IJWTEncryptionService<RunnerJWTEncryptionServiceOptionsType, Configuration> jwtTEncryptionService,
            IDateTimeService dateTimeService
        )
        {
            _jwtTEncryptionService = jwtTEncryptionService;
            _dateTimeService = dateTimeService;
        }

        public async Task GenerateJWT_Runs_DoesNotThrow(List<string> successLog)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                new Claim(ClaimTypes.Role, "User")
            };

            var jwt = _jwtTEncryptionService.GenerateJWT(claims, _dateTimeService.GetDateTimeUTC().AddDays(1), TokenType.Access);
            var claimsSerialized = JsonSerializer.Serialize(claims);

            successLog.Add($"JWT Created. \r\nClaimsInput:\r\n{claimsSerialized}\r\n \r\nJWT:\r\n{jwt}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task GenerateTokens_ClaimsInput_DoesNotThrow(List<string> successLog)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsSerialized = JsonSerializer.Serialize(claims);
            var generateTokensDescriptor = _jwtTEncryptionService.GenerateTokens(claims);
            var generateTokensDescriptorSerialized = JsonSerializer.Serialize(generateTokensDescriptor, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });


            successLog.Add($"JWT Created. \r\nClaimsInput:\r\n{claimsSerialized}\r\n\r\nGenerateTokensDescriptor:\r\n{generateTokensDescriptorSerialized}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task GenerateTokens_ExistingTokenAsInput_DoesNotThrow(List<string> successLog)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsSerialized = JsonSerializer.Serialize(claims);
            var generateTokensDescriptor = _jwtTEncryptionService.GenerateTokens(claims);
            var generateTokensDescriptorSerialized = JsonSerializer.Serialize(generateTokensDescriptor, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

            var generateTokensDescriptorFromExistingTokens = _jwtTEncryptionService.GenerateTokens(generateTokensDescriptor.Tokens.AccessToken, generateTokensDescriptor.Tokens.RefreshToken);
            var generateTokensDescriptorFromExistingTokensSerialized = JsonSerializer.Serialize(generateTokensDescriptorFromExistingTokens, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

            successLog.Add($"JWT Created. \r\nClaimsInput:\r\n{claimsSerialized}\r\n\r\nGenerateTokensDescriptor:\r\n{generateTokensDescriptorSerialized}\r\n\r\nGenerateTokensDescriptorFromExistingTokens:\r\n{generateTokensDescriptorFromExistingTokensSerialized}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task GetPrincipal_AccessAndRefreshTokenInputs_DoesNotThrow(List<string> successLog)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsSerialized = JsonSerializer.Serialize(claims);
            var generateTokensDescriptor = _jwtTEncryptionService.GenerateTokens(claims);
            var generateTokensDescriptorSerialized = JsonSerializer.Serialize(generateTokensDescriptor, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

            var principalFromAccessToken = _jwtTEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.AccessToken, true, TokenType.Access);
            var principalFromAccessTokenSerialized = JsonSerializer.Serialize(principalFromAccessToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

            var principalFromRefreshToken = _jwtTEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.RefreshToken, true, TokenType.Refresh);
            var principalFromRefreshTokenSerialized = JsonSerializer.Serialize(principalFromRefreshToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

            successLog.Add($"JWT Created. \r\nClaimsInput:\r\n{claimsSerialized}\r\n\r\nGenerateTokensDescriptor:\r\n{generateTokensDescriptorSerialized}\r\n\r\nprincipalFromAccessToken:\r\n{principalFromAccessTokenSerialized}\r\n\r\nprincipalFromRefreshToken:\r\n{principalFromRefreshTokenSerialized}\r\n");
            await Task.CompletedTask.ConfigureAwait(false);
        }
        //public async Task GenerateTokensAndGetPrincipal_Runs_TBD(List<string> successLog)
        //{

        //}

        //public async Task _Runs_CorrelationIdIsNotNull(List<string> successLog)
        ////Run application
        //var claims = new Claim[]
        //    {
        //            new Claim(ClaimTypes.NameIdentifier, "AccountId"),
        //            new Claim(ClaimTypes.Role, "User")
        //    };
        //    Console.WriteLine($"Claims: {JsonSerializer.Serialize(claims)}");
        //    Console.WriteLine();

        //    var jwt = jwtEncryptionService.GenerateJWT(claims, dateTimeService.GetDateTimeUTC().AddDays(1), Abstractions.Models.TokenType.Access);
        //    Console.WriteLine($"GenerateJWT (Claims): {JsonSerializer.Serialize(jwt)}");
        //    Console.WriteLine();

        //    var generateTokensDescriptor = jwtEncryptionService.GenerateTokens(claims);
        //    Console.WriteLine($"GenerateTokens (Claims): {JsonSerializer.Serialize(generateTokensDescriptor)}");
        //    Console.WriteLine();

        //    var tokens = jwtEncryptionService.GenerateTokens(generateTokensDescriptor.Tokens.AccessToken, generateTokensDescriptor.Tokens.RefreshToken);
        //    Console.WriteLine($"GenerateTokens (AccessToken, RefreshToken): {JsonSerializer.Serialize(tokens)}");
        //    Console.WriteLine();

        //    var principalFromAccessToken = jwtEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.AccessToken, true, Abstractions.Models.TokenType.Access);
        //    Console.WriteLine($"GetPrincipal - AccessToken: {JsonSerializer.Serialize(principalFromAccessToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true })}");
        //    Console.WriteLine();

        //    var principalFromRefreshToken = jwtEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.RefreshToken, true, Abstractions.Models.TokenType.Refresh);
        //    Console.WriteLine($"GetPrincipal - RefreshToken: {JsonSerializer.Serialize(principalFromRefreshToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true })}");
        //    Console.WriteLine();

        //    Assert.IsNotNull(correlationId, "CorrelationId is null");
        //    successLog.Add($"CorrelationId: {correlationId}");

        //    await Task.CompletedTask.ConfigureAwait(false);
        //}
    }
}
