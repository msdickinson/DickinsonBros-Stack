using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.MiddlewareService.ASP.Abstractions;
using Microsoft.AspNetCore.Http;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.MiddlewareService.ASP
{
    public class MiddlewareASPService : IMiddlewareASPService
    {
        internal const string CORRELATION_ID = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly IGuidService _guidService;
        internal readonly ILoggerService<MiddlewareASPService> _loggerService;
        internal readonly ICorrelationService _correlationService;

        public MiddlewareASPService(
            RequestDelegate next,
            IStopwatchFactory stopwatchFactory,
            IDateTimeService dateTimeService,
            ITelemetryWriterService telemetryWriterService,
            IGuidService guidService,
            ICorrelationService correlationService,
            ILoggerService<MiddlewareASPService> loggingService
        )
        {
            _next = next;
            _guidService = guidService;
            _loggerService = loggingService;
            _correlationService = correlationService;
            _stopwatchFactory = stopwatchFactory;
            _dateTimeService = dateTimeService;
            _telemetryWriterService = telemetryWriterService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
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

            var requestBody = await FormatRequestAsync(context.Request);
            var originalBodyStream = context.Response.Body;

            if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
            {
                _loggerService.LogInformationRedacted
                (
                    $"+ {insertTelemetryItem.Request}",
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { "Path", context.Request.Path.Value },
                        { "Method", context.Request.Method },
                        { "Scheme", context.Request.Scheme },
                        { "Prams", context.Request.Query.ToDictionary() },
                        { "Body", requestBody }
                    }
                );
            }

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            try
            {
                await _next(context);

                context.Response.Headers.TryAdd
                (
                    CORRELATION_ID,
                   _correlationService.CorrelationId
                );

                var responseBodyString = await FormatResponseAsync(context.Response);
                await responseBody.CopyToAsync(originalBodyStream);
                if (context.Request.Path.Value.Contains("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    _loggerService.LogInformationRedacted
                    (
                        $"Response {insertTelemetryItem.Request}",
                        Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                        new Dictionary<string, object>
                        {
                            { "Body", responseBodyString },
                            { "StatusCode", context.Response.StatusCode }
                        }
                    );
                }

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.Successful;
                }
                else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.CallerError;
                }
                else
                {
                    insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                }
                stopwatchService.Stop();

            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                context.Response.StatusCode = 500;
                context.Response.Headers.TryAdd
                (
                    CORRELATION_ID,
                   _correlationService.CorrelationId
                );
                insertTelemetryItem.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _loggerService.LogErrorRedacted
                (
                    $"Unhandled exception {insertTelemetryItem.Request}",
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { "StatusCode", context.Response.StatusCode }
                    }
                );
            }
            finally
            {
                insertTelemetryItem.Duration = stopwatchService.Elapsed;

                _loggerService.LogInformationRedacted
                (
                    $"- {insertTelemetryItem.Request}",
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { "Duration", insertTelemetryItem.Duration }
                    }
                );


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

        internal async Task<string> FormatResponseAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string body = await new StreamReader(response.Body).ReadToEndAsync();

            response.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }
    }
}
