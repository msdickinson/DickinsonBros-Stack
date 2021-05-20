using DickinsonBros.Core.Telemetry.Abstractions.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Telemetry.Abstractions
{
    public interface ITelemetryServiceWriter
    {
        void Insert(InsertTelemetryRequest telemetryItem);
        Task FlushAsync();
    }
}