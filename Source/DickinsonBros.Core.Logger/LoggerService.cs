using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Logger.Models;
using DickinsonBros.Core.Redactor.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DickinsonBros.Core.Logger
{
    public class LoggerService<T> : ILoggerService<T>
    {
        internal readonly ILogger<T> _logger;
        internal readonly IRedactorService _redactorService;
        internal readonly ICorrelationService _correlationService;

        public LoggerService(ILogger<T> logger, IRedactorService redactorService, ICorrelationService correlationService)
        {
            _logger = logger;
            _redactorService = redactorService;
            _correlationService = correlationService;
        }

        public void LogDebugRedacted(string message, LogGroup logGroup, IDictionary<string, object> properties = null)
        {
            Log(LogLevel.Debug, message, logGroup, properties, null);
        }

        public void LogInformationRedacted(string message, LogGroup logGroup, IDictionary<string, object> properties = null)
        {
            Log(LogLevel.Information, message, logGroup, properties, null);
        }

        public void LogWarningRedacted(string message, LogGroup logGroup, IDictionary<string, object> properties = null)
        {
            Log(LogLevel.Warning, message, logGroup, properties, null);
        }

        public void LogErrorRedacted(string message, LogGroup logGroup, Exception exception, IDictionary<string, object> properties = null)
        {
            Log(LogLevel.Error, message, logGroup, properties, exception);
        }

        public void Log(LogLevel logLevel, string message, LogGroup logGroup, IDictionary<string, object> properties = null, Exception exception = null)
        {
            var propertiesRedacted = new List<KeyValuePair<string, object>>();

            if (properties != null)
            {
                propertiesRedacted.AddRange
                (
                    properties.Select
                    (
                        property => new KeyValuePair<string, object>(property.Key, _redactorService.Redact(property.Value))
                    ).ToList()
                );
            }

            propertiesRedacted.Add(new KeyValuePair<string, object>("LogGroup", logGroup));

            if (_correlationService.CorrelationId != null)
            {
                propertiesRedacted.Add(new KeyValuePair<string, object>("CorrelationId", _correlationService.CorrelationId));
            }

            _logger.Log(logLevel, 1, ((object)(new LogState(propertiesRedacted))), exception, (_, ex) => Formatter(message, propertiesRedacted));
        }

        internal string Formatter(string message, IList<KeyValuePair<string, object>> propertiesRedacted)
        {
            return message +
                    Environment.NewLine +
                    String.Concat
                    (
                        propertiesRedacted.Select
                        (
                            keyValuePair => $"{keyValuePair.Key}: {keyValuePair.Value}" +
                                            Environment.NewLine
                        )
                    );
        }

    }
}
