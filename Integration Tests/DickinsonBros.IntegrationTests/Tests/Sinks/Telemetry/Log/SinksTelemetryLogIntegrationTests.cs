using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Test.Integration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.Log
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "SinksTelemetryLog", Group = "Sinks")]
    public class SinksTelemetryLogIntegrationTests : ISinksTelemetryLogIntegrationTests
    {
        private readonly ITelemetryWriterService _telemetryWriterService;
        private readonly ISinksTelemetryLogService _sinksTelemetryLogService;
        public SinksTelemetryLogIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService,
            ISinksTelemetryLogService sinksTelemetryLogService
        )
        {
            _telemetryWriterService = telemetryWriterService;
            _sinksTelemetryLogService = sinksTelemetryLogService;
        }

        public async Task Insert_Runs_DoesNotThrow(List<string> successLog)
        {
            var insertTelemetryItem = new InsertTelemetryItem()
            {
                DateTimeUTC = DateTime.UtcNow,
                ConnectionName = "SampleConnectionName",
                Duration = TimeSpan.FromSeconds(1),
                Request = "SampleSignalRequest",
                CorrelationId = "SampleCorrelationId",
                TelemetryResponseState = TelemetryResponseState.Successful,
                TelemetryType = TelemetryType.Application
            };

            _telemetryWriterService.Insert(insertTelemetryItem);

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
