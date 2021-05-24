using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions
{
    public interface ISinksTelemetryAzureTablesService<T>
    where T : AzureTableServiceOptionsType
    {
        Task FlushAsync();
    }
}
