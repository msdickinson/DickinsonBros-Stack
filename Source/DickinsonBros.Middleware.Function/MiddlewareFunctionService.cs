using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Middleware.Function.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dickinsonbros.Middleware.Function
{

    public class MiddlewareFunctionService<T, U> : IMiddlewareFunctionService
    where T : JWTEncryptionServiceOptionsType
    where U : CertificateEncryptionServiceOptionsType
    {
        internal const string CORRELATION_ID = "X-Correlation-ID";
        internal const string TOKEN_TYPE = "Bearer";

        internal readonly IServiceProvider _serviceProvider;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly IGuidService _guidService;
        internal readonly ILoggerService<MiddlewareFunctionService<T, U>> _loggingService;
        internal readonly ICorrelationService _correlationService;
        internal readonly IJWTEncryptionService<T, U> _jwtEncryptionService;
        internal readonly IStopwatchFactory _stopwatchFactory;

        public MiddlewareFunctionService(
            IServiceProvider serviceProvider,
            IDateTimeService dateTimeService,
            ITelemetryWriterService telemetryWriterService,
            IGuidService guidService,
            ICorrelationService correlationService,
            ILoggerService<MiddlewareFunctionService<T, U>> loggingService,
            IJWTEncryptionService<T, U> jwtEncryptionService,
            IStopwatchFactory stopwatchFactory
        )
        {
            _guidService = guidService;
            _loggingService = loggingService;
            _correlationService = correlationService;
            _serviceProvider = serviceProvider;
            _dateTimeService = dateTimeService;
            _telemetryWriterService = telemetryWriterService;
            _jwtEncryptionService = jwtEncryptionService;
            _stopwatchFactory = stopwatchFactory;
        }

        public async Task<ContentResult> InvokeAsync(HttpContext context, Func<Task<ContentResult>> callback)
        {
            return await InvokeAsync(context, null, callback, false).ConfigureAwait(false);
        }

        public async Task<ContentResult> InvokeWithJWTAuthAsync(HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callback, params string[] roles)
        {
            return await InvokeAsync(context, callback, null, true, roles).ConfigureAwait(false);
        }

        internal async Task<ContentResult> InvokeAsync(HttpContext context, Func<ClaimsPrincipal, Task<ContentResult>> callbackClaimsPrincipal, Func<Task<ContentResult>> callback, bool withAuth, params string[] roles)
        {
            var contentResult = (ContentResult)null;
            var insertTelemetryItem = new InsertTelemetryItem
            {
                ConnectionName = $"{context.Request.Host}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"{context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                TelemetryType = TelemetryType.Application
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            _correlationService.CorrelationId = EnsureCorrelationId(context.Request);
            insertTelemetryItem.CorrelationId = _correlationService.CorrelationId;
            _telemetryWriterService.ScopedUserStory = insertTelemetryItem.Request;

            var requestBody = await FormatRequestAsync(context.Request);

            try
            {
                context.Response.Headers.TryAdd
                (
                    CORRELATION_ID,
                   _correlationService.CorrelationId
                );

                bool vaildAuth = !withAuth;
                var user = (ClaimsPrincipal)null;
                var role = (string)null;
                var nameIdentifier = (string)null;
                if (withAuth)
                {
                    string token = context.Request.Headers.FirstOrDefault(header => header.Key == "Authorization").Value.ToString().Split("Bearer").LastOrDefault().Trim();
                    var accessTokenClaims = _jwtEncryptionService.GetPrincipal(token, true, TokenType.Access);

                    if
                    (
                        accessTokenClaims == null ||
                        !accessTokenClaims.Identity.IsAuthenticated ||
                        !accessTokenClaims.Claims.Any(claim => claim.Type == ClaimTypes.Role && roles.Any(role => role.ToString() == claim.Value)) ||
                        !accessTokenClaims.Claims.Any(claim => claim.Type == ClaimTypes.NameIdentifier)
                    )
                    {
                        contentResult = new ContentResult
                        {
                            StatusCode = 401
                        };
                    }
                    else
                    {
                        role = accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
                        nameIdentifier = accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                        user = accessTokenClaims;
                        vaildAuth = true;
                    }
                }

                if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    _loggingService.LogInformationRedacted
                    (
                        $"+ {insertTelemetryItem.Request}",
                        LogGroup.Infrastructure,
                        new Dictionary<string, object>
                        {
                            { "Path", context.Request.Path.Value },
                            { "Method", context.Request.Method },
                            { "Scheme", context.Request.Scheme },
                            { "Prams", context.Request.Query.ToDictionary() },
                            { "Body", requestBody },
                            { "Role", role},
                            { "NameIdentifier",  nameIdentifier}
                        }
                    );
                }

                if (vaildAuth)
                {
                    if (withAuth)
                    {
                        contentResult = await callbackClaimsPrincipal(user).ConfigureAwait(false);
                    }
                    else
                    {
                        contentResult = await callback().ConfigureAwait(false);
                    }
                }

                stopwatchService.Stop();

                if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    _loggingService.LogInformationRedacted
                    (
                        $"Response {insertTelemetryItem.Request}",
                        LogGroup.Infrastructure,
                        new Dictionary<string, object>
                        {
                            { "ContentType", contentResult.ContentType },
                            { "Body", contentResult.Content },
                            { "StatusCode", contentResult.StatusCode }
                        }
                    );
                }

                if (contentResult.StatusCode >= 200 && contentResult.StatusCode < 300)
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.Successful;
                }
                else if (contentResult.StatusCode >= 400 && contentResult.StatusCode < 500)
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.CallerError;
                }
                else
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.ReciverError;
                }

                return contentResult;
            }
            catch (Exception exception)
            {
                contentResult = new ContentResult
                {
                    StatusCode = 500
                };
                insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                stopwatchService.Stop();

                _loggingService.LogErrorRedacted
                (
                    $"Unhandled exception {insertTelemetryItem.Request}",
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { "StatusCode", contentResult.StatusCode }
                    }
                );

                return contentResult;
            }
            finally
            {
                insertTelemetryItem.Duration = stopwatchService.Elapsed;

                if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    _loggingService.LogInformationRedacted
                    (
                        $"- {insertTelemetryItem.Request}",
                        LogGroup.Infrastructure,
                        new Dictionary<string, object>
                        {
                        { "ElapsedMilliseconds", insertTelemetryItem.Duration }
                        }
                    );
                }
                _telemetryWriterService.Insert(insertTelemetryItem);
            }
        }


        internal string EnsureCorrelationId(HttpRequest request)
        {
            if (!request.Headers.Any(e => e.Key == CORRELATION_ID))
            {
                return _guidService.NewGuid().ToString();
            }

            return request.Headers.First(e => e.Key == CORRELATION_ID).Value;
        }

        internal async Task<string> FormatRequestAsync(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var body = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return body;
        }


    }
}
