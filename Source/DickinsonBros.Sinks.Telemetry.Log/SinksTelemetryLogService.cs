using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.Log
{
    public class SinksTelemetryLogService : ITelemetryServiceWriter
    {
        internal readonly ILoggerService<SinksTelemetryLogService> _loggerService;
        public SinksTelemetryLogService
        (
            ILoggerService<SinksTelemetryLogService> loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async Task FlushAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task InsertAsync(InsertTelemetryRequest telemetryItem)
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

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
