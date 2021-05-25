using Microsoft.Azure.Cosmos.Table;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryDataEntity : TableEntity
    {
        public string Source { get; set; }
        public string TelemetryType { get; set; }
        public string Connection { get; set; }
        public int ElapsedMilliseconds { get; set; }
        public DateTime EventTimestamp { get; set; }
        public string TelemetryState { get; set; }
        public string Request { get; set; }
        public string CorrelationId { get; set; }
    }
}
