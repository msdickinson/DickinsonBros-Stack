using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.Infrastructure.Rest.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Rest
{
    public class RestService : IRestService
    {
        internal readonly IServiceProvider _serviceProvider;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly ICorrelationService _correlationService;
        internal readonly IGuidService _guidService;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ILoggerService<RestService> _loggingService;
        internal const string CORRELATION_ID = "X-Correlation-ID";
        internal readonly string DURABLE_REST_MESSAGE_REQUEST = $"{nameof(RestService)} - Request";
        internal readonly string DURABLE_REST_MESSAGE_RESPONSE = $"{nameof(RestService)} - Response";

        public RestService
        (
            IServiceProvider serviceProvider,
            IGuidService guidService,
            IDateTimeService dateTimeService,
            ITelemetryWriterService telemetryWriterService,
            ICorrelationService correlationService,
            IStopwatchFactory stopwatchFactory,
            ILoggerService<RestService> loggingService
        )
        {
            _correlationService = correlationService;
            _loggingService = loggingService;
            _dateTimeService = dateTimeService;
            _telemetryWriterService = telemetryWriterService;
            _guidService = guidService;
            _stopwatchFactory = stopwatchFactory;
            _serviceProvider = serviceProvider;
        }

     
        public async Task<HttpResponse<T>> ExecuteAsync<T>
        (
            string connectionName,
            HttpClient httpClient,
            HttpRequestMessage httpRequestMessage,
            int retrys,
            double timeoutInSeconds
        )
        {
            var httpResponseMessage = await ExecuteAsync(connectionName, httpClient, httpRequestMessage, retrys, timeoutInSeconds).ConfigureAwait(false);

            return new HttpResponse<T>
            {
                HttpResponseMessage = httpResponseMessage,
                Data = (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.Content != null) ?
                            JsonSerializer.Deserialize<T>(
                                await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false),
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                })
                            : default
            };
        }

        public async Task<HttpResponseMessage> ExecuteAsync
        (
            string connectionName,
            HttpClient httpClient,
            HttpRequestMessage httpRequestMessage,
            int retrys,
            double timeoutInSeconds
        )
        {
            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            var attempts = 0;

            HttpResponseMessage httpResponseMessage = null;

            if (!httpRequestMessage.Headers.Any(e => e.Key == CORRELATION_ID))
            {
                httpRequestMessage.Headers.Add
                (
                    CORRELATION_ID,
                    !string.IsNullOrWhiteSpace(_correlationService.CorrelationId) ? _correlationService.CorrelationId : _guidService.NewGuid().ToString()
                );
            }

            do
            {
                var insertTelemetryRequest = new InsertTelemetryItem
                {
                    ConnectionName = connectionName,
                    DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                    Request = httpRequestMessage.Method.Method,
                    TelemetryType = TelemetryType.Rest,
                    CorrelationId = _correlationService.CorrelationId
                };

                await LogRequest(httpRequestMessage, httpClient, attempts).ConfigureAwait(false);
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutInSeconds));
                stopwatchService.Start();

                try
                {
                    httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cts.Token).ConfigureAwait(false);
                    stopwatchService.Stop();
                }
                catch (OperationCanceledException)
                {
                    httpResponseMessage = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.GatewayTimeout
                    };
                    stopwatchService.Stop();
                }
                finally
                {
                    insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                    insertTelemetryRequest.TelemetryResponseState = DetermineTelemetryResponseState((int)httpResponseMessage.StatusCode);
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }

                attempts++;
                await LogResponse(httpRequestMessage, httpResponseMessage, httpClient, attempts, (int)stopwatchService.ElapsedMilliseconds).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    break;
                }
                httpRequestMessage = await CloneAsync(httpRequestMessage).ConfigureAwait(false);
            } while (retrys >= attempts);

            return httpResponseMessage;
        }

        public async Task LogRequest
        (
            HttpRequestMessage httpRequestMessage,
            HttpClient httpClient,
            int attempts
        )
        {
            var properties = new Dictionary<string, object>
            {
                { "Attempts", attempts },
                { "BaseUrl", httpClient.BaseAddress },
                { "Resource", httpRequestMessage.RequestUri.OriginalString },
                { "RequestContent", httpRequestMessage.Content != null ? await httpRequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null },
            };

            _loggingService.LogInformationRedacted
            (
                DURABLE_REST_MESSAGE_REQUEST,
                LogGroup.Infrastructure,
                properties
            );
        }

        public async Task LogResponse
        (
            HttpRequestMessage httpRequestMessage,
            HttpResponseMessage httpResponseMessage,
            HttpClient httpClient,
            int attempts,
            int elapsedMilliseconds
        )
        {
            var properties = new Dictionary<string, object>
            {
                { "Attempts", attempts },
                { "BaseUrl", httpClient.BaseAddress },
                { "Resource", httpRequestMessage.RequestUri.LocalPath },
                { "RequestContent", httpRequestMessage.Content != null ? await httpRequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null },
                { "ResponseContent", httpResponseMessage?.Content != null ? await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null },
                { "ElapsedMilliseconds", elapsedMilliseconds },
                { "StatusCode", httpResponseMessage?.StatusCode }
            };

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                _loggingService.LogWarningRedacted
                (
                    DURABLE_REST_MESSAGE_RESPONSE,
                    LogGroup.Infrastructure,
                    properties
                );
                return;
            }

            _loggingService.LogInformationRedacted
            (
                DURABLE_REST_MESSAGE_RESPONSE,
                LogGroup.Infrastructure,
                properties
            );

        }

        public TelemetryResponseState DetermineTelemetryResponseState(int statusCode)
        {
            if (statusCode >= 100 && statusCode < 400)
            {
                return TelemetryResponseState.Successful;
            }

            else if (statusCode >= 400 && statusCode < 500)
            {
                return TelemetryResponseState.CallerError;
            }

            return TelemetryResponseState.UnhandledException;
        }

        internal async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = await CloneAsync(request.Content).ConfigureAwait(false),
                Version = request.Version
            };
            foreach (KeyValuePair<string, object> prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        internal async Task<HttpContent> CloneAsync(HttpContent content)
        {
            if (content == null) return null;

            var ms = new MemoryStream();
            await content.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;

            var clone = new StreamContent(ms);
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }
            return clone;
        }
    }
}
