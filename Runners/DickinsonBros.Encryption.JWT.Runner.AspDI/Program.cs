using BaseRunner;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.JWT.Runner.AspDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                //Start application
                using var provider = serviceCollection.BuildServiceProvider();
                var jwtEncryptionService = provider.GetRequiredService<IJWTEncryptionService<Models.Runner, Configuration>>();
                var dateTimeService = provider.GetRequiredService<IDateTimeService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run application
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

                //Stop application
                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private IServiceCollection ConfigureServices()
        {
            var configuration = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configuration);

            serviceCollection.AddLoggerService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddDateTimeService();
            serviceCollection.AddCertificateEncryptionService<Configuration>();
            serviceCollection.AddJWTEncryptionService<Models.Runner, Configuration>();

            return serviceCollection;
        }
    }
}
