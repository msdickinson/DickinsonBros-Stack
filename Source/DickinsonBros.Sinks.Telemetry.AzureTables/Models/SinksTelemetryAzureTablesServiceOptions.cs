using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Models
{
    [ExcludeFromCodeCoverage]
    public class SinksTelemetryAzureTablesServiceOptions
    {
        public TimeSpan UploadInterval { get; set; }
        public string TableName { get; set; }
        public string PartitionKey { get; set; }
    }
}
