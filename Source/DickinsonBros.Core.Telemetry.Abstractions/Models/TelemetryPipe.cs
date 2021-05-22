using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryPipe
    {
        public string ApplicationName { get; set; }
        public string TelemetryType { get; set; }
        public string ConnectionName { get; set; }
        public string SignalRequest { get; set; }
        public IEnumerable<TelemetrySignal> Signals { get; set; }
    }
}
