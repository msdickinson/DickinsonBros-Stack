using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Sinks.Telemetry.AzureTables
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "SinksTelemetryAzureTables", Group = "Sinks")]
    public class SinksTelemetryAzureTablesIntegrationTests : ISinksTelemetryAzureTablesIntegrationTests
    {
        public ITelemetryServiceWriter _telemetryServiceWriter;
        public SinksTelemetryAzureTablesIntegrationTests
        (
            ITelemetryServiceWriter telemetryServiceWriter
        )
        {
            _telemetryServiceWriter = telemetryServiceWriter;
        }

        public async Task InsertAndFlush_Runs_DoesNotThrow(List<string> successLog)
        {
            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = "SampleConnectionName",
                DateTimeUTC = DateTime.UtcNow,
                Duration = TimeSpan.FromSeconds(100),
                SignalRequest = "SampleSignalRequest",
                SignalResponse = "SampleSignalResponse",
                TelemetryResponseState = TelemetryResponseState.Successful,
                TelemetryType = TelemetryType.Application
            };

            await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            await _telemetryServiceWriter.FlushAsync().ConfigureAwait(false);
        }

    }
}
