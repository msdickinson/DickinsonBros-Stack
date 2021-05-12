using System.Collections.Generic;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public class TelemetryPipe
    {
        public string ApplicationName { get; set; }
        public string TelemetryType { get; set; }
        public string ConnectionName { get; set; }
        public string SignalRequest { get; set; }
        public IEnumerable<TelemetrySignal> Signals { get; set; }
    }
}
