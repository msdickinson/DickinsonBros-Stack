using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Encryption.AES.Abstractions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public ITelemetryServiceWriter _telemetryServiceWriter;
        public SinksTelemetryLogIntegrationTests
        (
            ITelemetryServiceWriter telemetryServiceWriter
        )
        {
            _telemetryServiceWriter = telemetryServiceWriter;
        }

        public async Task InsertAsync_Runs_DoesNotThrow(List<string> successLog)
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
        }

    }
}
