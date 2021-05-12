namespace DickinsonBros.Core.Telemetry.Abstractions.Models
{
    public enum TelemetryResponseState
    {
        Successful      = 0,
        CallerError     = 1,
        ReciverError    = 2,
        UnHandledException     = 3
    }
}
