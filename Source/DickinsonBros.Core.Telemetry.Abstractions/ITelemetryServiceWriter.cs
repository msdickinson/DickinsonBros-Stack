using DickinsonBros.Core.Telemetry.Abstractions.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Telemetry.Abstractions
{
    public interface ITelemetryServiceWriter
    {
        Task InsertAsync(InsertTelemetryRequest telemetryItem);
        Task FlushAsync();
    }
}