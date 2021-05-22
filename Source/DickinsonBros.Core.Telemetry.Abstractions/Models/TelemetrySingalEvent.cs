using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetrySingalEvent
    {
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
