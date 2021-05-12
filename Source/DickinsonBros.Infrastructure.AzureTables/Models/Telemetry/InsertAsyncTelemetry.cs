using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System;

namespace DickinsonBros.Infrastructure.AzureTables.Models.Telemetry
{
    public class InsertAsyncTelemetry
    {
        public object Item { get; internal set; }
        public string TableName { get; internal set; }
        public Func<DateTime> DateTimeUTC { get; internal set; }
        public TableResult<object> TableResult { get; internal set; }
        public Exception Exception { get; internal set; }
        public TimeSpan Duration { get; internal set; }
    }
}
