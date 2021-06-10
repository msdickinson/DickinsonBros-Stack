namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public enum TelemetryType
    {
        Application     = 0,
        AzureTable      = 1,
        Cosmos          = 2,
        SMTP            = 3,
        FileSystem      = 4,
        Rest            = 5,
        SQL             = 6,
        DomainNameServer = 7,
    }
}
