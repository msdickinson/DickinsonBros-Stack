using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TelemetryWriterServiceOptions
    {
        public string ApplicationName { get; set; }
    }
}
