using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using System.Collections.Generic;

namespace DickinsonBros.Sinks.Telemetry.Log
{
    public class SinksTelemetryLogService : ISinksTelemetryLogService
    {
        internal readonly ILoggerService<SinksTelemetryLogService> _loggerService;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        public SinksTelemetryLogService
        (
            ILoggerService<SinksTelemetryLogService> loggerService,
            ITelemetryWriterService telemetryWriterService
        )
        {
            _loggerService = loggerService;
            _telemetryWriterService = telemetryWriterService;

            telemetryWriterService.NewTelemetryEvent += TelemetryWriterService_NewTelemetryEvent;
        }

        internal void TelemetryWriterService_NewTelemetryEvent(Core.Telemetry.Abstractions.Models.TelemetryItem telemetryItem)
        {
            _loggerService.LogInformationRedacted
            (
                $"SinksTelemetryLogService.InsertTelemetryRequest",
                Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                new Dictionary<string, object>
                {
                    { nameof(telemetryItem), telemetryItem }
                }
            );
        }
    }
}
