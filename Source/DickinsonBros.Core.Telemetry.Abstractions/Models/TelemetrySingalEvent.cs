using System;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public class TelemetrySingalEvent
    {
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
