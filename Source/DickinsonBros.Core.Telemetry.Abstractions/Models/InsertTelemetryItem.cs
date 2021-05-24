using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class InsertTelemetryItem
    {
        public TelemetryType TelemetryType { get; set; }
        public string ConnectionName { get; set; }
        public string Request { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public TimeSpan Duration { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
        public string CorrelationId { get; set;  }
    }
}
