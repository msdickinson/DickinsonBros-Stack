using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions
{
    interface ISinksTelemetryAzureTables<U>
    where U : AzureTableServiceOptionsType
    {
    }
}
