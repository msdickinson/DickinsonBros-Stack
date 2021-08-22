using DickinsonBros.Core.Telemetry.Abstractions.Models;
using System.Threading;

namespace DickinsonBros.Core.Telemetry.Abstractions
{
    public interface ITelemetryWriterService
    {
        public string ScopedUserStory { get; set; }

        void Insert(InsertTelemetryItem telemetryItem);

        event NewTelemetryItemEventHandler NewTelemetryEvent;
        public delegate void NewTelemetryItemEventHandler(TelemetryItem telemetryItem);

    }
}