using System.Collections.Generic;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public class TelemetrySignal
    {
        public string SignalResponse { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
        public IEnumerable<TelemetrySingalEvent> SignalEvents { get; set; }
    }
}
