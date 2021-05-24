using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Logger
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Telemetry", Group = "Core")]
    public class TelemetryIntegrationTests : ITelemetryIntegrationTests
    {
        internal readonly ITelemetryWriterService _telemetryWriterService;

        public TelemetryIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService
        )
        {
            _telemetryWriterService = telemetryWriterService;
        }

        public async Task Insert_Runs_TelemetryNewEventFired(List<string> successLog)
        {
            var telemetryItems = new List<TelemetryItem>();
            var telemetryItem = new InsertTelemetryItem()
            {
                DateTimeUTC = System.DateTime.UtcNow,
                ConnectionName = "SampleConnectionName",
                Duration = TimeSpan.FromSeconds(1),
                Request = "SampleSignalRequest",
                CorrelationId = "SampleCorrelationId",
                TelemetryResponseState = TelemetryResponseState.Successful,
                TelemetryType = TelemetryType.Application
            };

            _telemetryWriterService.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };
            _telemetryWriterService.Insert(telemetryItem);

            Assert.AreEqual(1, telemetryItems.Count, "TelemetryItem not found");
            successLog.Add($"TelemetryItem Inserted");

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
