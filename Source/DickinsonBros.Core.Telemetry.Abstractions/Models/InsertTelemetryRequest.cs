using System;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public class InsertTelemetryRequest
    {
        public TelemetryType TelemetryType { get; set; }   
        public string ConnectionName { get; set; }      
        public string SignalRequest { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public TimeSpan Duration { get; set; }
        public string SignalResponse { get; set; }
        public TelemetryResponseState TelemetryResponseState { get; set; }
    }
}
