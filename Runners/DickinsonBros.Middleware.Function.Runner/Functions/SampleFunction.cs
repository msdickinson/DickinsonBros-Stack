using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Middleware.Function.Abstractions;
using DickinsonBros.Middleware.Function.Runner.Config;
using DickinsonBros.Middleware.Function.Runner.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dickinsonbros.Middleware.Function.Runner.Functions
{
    public class SampleFunction
    {
        #region constants

        internal const string USER = "User";
        internal const string FUNCTION_NAME = "SampleFunction";
        internal const string FUNCTION_NAME_WITH_AUTH = "SampleFunctionWithAuth";
        internal const string FUNCTION_NAME_FETCH_AUTH = "SampleFunctionFetchAuth";

        internal const int TWO_HOURS_IN_SECONDS = 7200;
        internal const string BearerTokenType = "Bearer";
        #endregion

        #region members
        internal readonly ILoggerService<SampleFunction> _loggingService;
        internal readonly IMiddlewareFunctionService _middlewareService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly IJWTEncryptionService<DickinsonBros.Middleware.Function.Runner.Config.Runner, Configuration> _jwtEncryptionService;
        #endregion

        #region .ctor
        public SampleFunction
        (
            ILoggerService<SampleFunction> loggingService,
            IDateTimeService dateTimeService,
            IMiddlewareFunctionService middlewareService,
            IJWTEncryptionService<DickinsonBros.Middleware.Function.Runner.Config.Runner, Configuration> jwtEncryptionService,
            ISinksTelemetryLogService sinksTelemetryLogService,
            ISinksTelemetryAzureTablesService<StorageAccountDickinsonBros> sinksTelemetryAzureTablesService
        )
        {
            _loggingService = loggingService;
            _dateTimeService = dateTimeService;
            _middlewareService = middlewareService;
            _jwtEncryptionService = jwtEncryptionService;
        }
        #endregion

        #region function
        [FunctionName(FUNCTION_NAME)]
        public async Task<ContentResult> RunAsync
        (
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FUNCTION_NAME)] HttpRequest req
        )
        {
            return await _middlewareService.InvokeAsync
            (
                req.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = "{}",
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }


        [FunctionName(FUNCTION_NAME_WITH_AUTH)]
        public async Task<ContentResult> RunWithAuthAsync
        (
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FUNCTION_NAME_WITH_AUTH)] HttpRequest req
        )
        {
            return await _middlewareService.InvokeWithJWTAuthAsync
            (
                req.HttpContext,
                async (user) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = "{}",
                        ContentType = "application/json"
                    };
                },
                USER
            ).ConfigureAwait(false);
        }

        [FunctionName(FUNCTION_NAME_FETCH_AUTH)]
        public async Task<ContentResult> FetchAuthAsync
        (
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = FUNCTION_NAME_FETCH_AUTH)] HttpRequest req
        )
        {
            return await _middlewareService.InvokeAsync
            (
                req.HttpContext,
                async () =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);

                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                        new Claim(ClaimTypes.Role, "User")
                    };
                    var accessToken = _jwtEncryptionService.GenerateJWT(claims, _dateTimeService.GetDateTimeUTC().AddHours(2), TokenType.Access);

                    var jwtResponse = new JWTResponse
                    {
                        AccessToken = accessToken,
                        AccessTokenExpiresIn = TWO_HOURS_IN_SECONDS,
                        TokenType = BearerTokenType
                    };

                    return new ContentResult
                    {
                        StatusCode = 200,
                        Content = System.Text.Json.JsonSerializer.Serialize(jwtResponse, new JsonSerializerOptions { WriteIndented = true }),
                        ContentType = "application/json"
                    };
                }
            ).ConfigureAwait(false);
        }

        #endregion

    }
}