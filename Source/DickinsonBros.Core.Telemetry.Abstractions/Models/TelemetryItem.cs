using System;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public class TelemetryItem
    {
        public string ApplicationName { get; set; }
        public string TelemetryType { get; set; }
        public string ConnectionName { get; set; }
        public string SignalRequest { get; set; }
        public string SignalResponse { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
