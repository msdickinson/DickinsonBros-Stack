using DickinsonBros.Core.Telemetry.Abstractions.Models;

namespace DickinsonBros.Core.Telemetry.Abstractions
{
    public interface ITelemetryWriterService
    {
        void Insert(InsertTelemetryItem telemetryItem);

        event NewTelemetryItemEventHandler NewTelemetryEvent;
        public delegate void NewTelemetryItemEventHandler(TelemetryItem telemetryItem);

    }
}