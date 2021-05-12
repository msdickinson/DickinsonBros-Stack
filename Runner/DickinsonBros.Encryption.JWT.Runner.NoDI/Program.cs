using DickinsonBros.Core.Correlation;
using DickinsonBros.Core.DateTime;
using DickinsonBros.Core.Logger;
using DickinsonBros.Core.Redactor;
using DickinsonBros.Core.Redactor.Models;
using DickinsonBros.Encryption.Certificate;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Models;
using DickinsonBros.Encryption.JWT.Models;
using DickinsonBros.Encryption.JWT.Runner.NoDI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.JWT.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var certificateEncryptionService = new CertificateEncryptionService<Configuration>(FetchCertificateEncryptionServiceOptions());
            var dateTimeService = new DateTimeService();
            var correlationService = new CorrelationService();
            var redactorService = new RedactorService(FetchRunnerRedactorServiceOptions());
            var baseLogger = new Logger<JWTEncryptionService<RunnerJWTEncryptionServiceOptions, Configuration>>(LoggerFactory.Create(builder => builder.AddConsole()));
            var loggerService = new LoggerService<JWTEncryptionService<RunnerJWTEncryptionServiceOptions, Configuration>>(baseLogger, redactorService, correlationService);
            var jwtEncryptionService = new JWTEncryptionService<RunnerJWTEncryptionServiceOptions, Configuration>
            (
                FetchRunnerJWTEncryptionServiceOptions(),
                dateTimeService,
                loggerService,
                certificateEncryptionService
            );


            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                new Claim(ClaimTypes.Role, "User")
            };
            Console.WriteLine($"Claims: {JsonSerializer.Serialize(claims)}");
            Console.WriteLine();

            var jwt = jwtEncryptionService.GenerateJWT(claims, dateTimeService.GetDateTimeUTC().AddDays(1), Abstractions.Models.TokenType.Access);
            Console.WriteLine($"GenerateJWT (Claims): {JsonSerializer.Serialize(jwt)}");
            Console.WriteLine();

            var generateTokensDescriptor = jwtEncryptionService.GenerateTokens(claims);
            Console.WriteLine($"GenerateTokens (Claims): {JsonSerializer.Serialize(generateTokensDescriptor)}"); 
            Console.WriteLine();

            var tokens = jwtEncryptionService.GenerateTokens(generateTokensDescriptor.Tokens.AccessToken, generateTokensDescriptor.Tokens.RefreshToken);
            Console.WriteLine($"GenerateTokens (AccessToken, RefreshToken): {JsonSerializer.Serialize(tokens)}"); 
            Console.WriteLine();

            var principalFromAccessToken = jwtEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.AccessToken, true, Abstractions.Models.TokenType.Access);
            Console.WriteLine($"GetPrincipal - AccessToken: {JsonSerializer.Serialize(principalFromAccessToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true })}");
            Console.WriteLine();

            var principalFromRefreshToken = jwtEncryptionService.GetPrincipal(generateTokensDescriptor.Tokens.RefreshToken, true, Abstractions.Models.TokenType.Refresh);
            Console.WriteLine($"GetPrincipal - RefreshToken: {JsonSerializer.Serialize(principalFromRefreshToken, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true })}"); 
            Console.WriteLine();

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private IOptions<RedactorServiceOptions> FetchRunnerRedactorServiceOptions()
        {
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[]
                {
                    "Password"
                },
                RegexValuesToRedact = new string[]
                {
                }
            };
            return Options.Create(redactorServiceOptions);
        }


        private IOptions<CertificateEncryptionServiceOptions<Configuration>> FetchCertificateEncryptionServiceOptions()
        {
            var certificateEncryptionServiceOptions = new CertificateEncryptionServiceOptions<Configuration>
            {
                ThumbPrint = "D253E5AF8C117821188A097682F3817E16FE9761",
                StoreLocation = "LocalMachine"
            };

            return Options.Create(certificateEncryptionServiceOptions);
        }

        public IOptions<JWTEncryptionServiceOptions<RunnerJWTEncryptionServiceOptions>> FetchRunnerJWTEncryptionServiceOptions()
        {
            var jwtEncryptionServiceOptions = new JWTEncryptionServiceOptions<RunnerJWTEncryptionServiceOptions>
            {
                AccessExpiresAfterMinutes = 30,
                Audience = "Website",
                RefershExpiresAfterMinutes = 90,
                Issuer = "Account"
            };

            return Options.Create(jwtEncryptionServiceOptions);
        }
    }
}
