using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryItem
    {
        public string Request { get; set; }
        public string Source { get; set; }
        public TelemetryType TelemetryType { get; set; }
        public string Connection { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public TimeSpan Duration { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
        public string CorrelationId { get; set; }
        public string UserStory { get; set; }
    }
}
