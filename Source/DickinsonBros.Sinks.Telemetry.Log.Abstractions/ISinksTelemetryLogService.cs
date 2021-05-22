using DickinsonBros.Core.Telemetry.Abstractions.Models;

namespace DickinsonBros.Sinks.Telemetry.Log.Abstractions
{
    interface ISinksTelemetryLogService
    {
        void Insert(TelemetryItem telemetryItem);
    }
}
