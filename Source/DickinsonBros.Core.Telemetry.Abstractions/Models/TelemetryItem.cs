using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryItem
    {
        public string ApplicationName { get; set; }
        public TelemetryType TelemetryType { get; set; }
        public string ConnectionName { get; set; }
        public string SignalRequest { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public TimeSpan Duration { get; set; }
        public string SignalResponse { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
    }
}
