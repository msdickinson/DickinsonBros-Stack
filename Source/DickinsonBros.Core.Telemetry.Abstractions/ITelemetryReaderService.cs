using DickinsonBros.Core.Telemetry.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Telemetry.Abstractions
{
    public interface ITelemetryReaderService
    {
        Task<IEnumerable<TelemetryPipe>> FetchAsync(string applicationName, TelemetryType telemetryType, DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<TelemetryPipe>> FetchAsync(string applicationName, DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<TelemetryPipe>> FetchAsync(TelemetryType telemetryType, DateTime startDateTime, DateTime endDateTime);
    }
}