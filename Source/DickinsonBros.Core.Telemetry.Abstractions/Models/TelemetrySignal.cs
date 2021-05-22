using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetrySignal
    {
        public string SignalResponse { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
        public IEnumerable<TelemetrySingalEvent> SignalEvents { get; set; }
    }
}
