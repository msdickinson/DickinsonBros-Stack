namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public enum TelemetryResponseState
    {
        Successful             = 0,
        NotFound               = 1,
        Conflict               = 2,
        CallerError            = 3,
        ReciverError           = 4,
        UnhandledException     = 5
    }
}
