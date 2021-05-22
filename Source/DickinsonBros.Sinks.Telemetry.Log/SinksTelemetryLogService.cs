using DickinsonBros.Core.Logger.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.Log
{
    //public class SinksTelemetryLogService : ISinksTelemetryLogService
    //{
    //    internal readonly ILoggerService<SinksTelemetryLogService> _loggerService;
    //    public SinksTelemetryLogService
    //    (
    //        ILoggerService<SinksTelemetryLogService> loggerService
    //    )
    //    {
    //        _loggerService = loggerService;
    //    }

    //    public void Insert(Core.Telemetry.Abstractions.Models.TelemetryItem telemetryItem)
    //    {
    //        _loggerService.LogInformationRedacted
    //        (
    //            $"SinksTelemetryLogService.InsertTelemetryRequest",
    //            Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
    //            new Dictionary<string, object>
    //            {
    //                { nameof(telemetryItem), telemetryItem }
    //            }
    //        );
    //    }
    //}
}
