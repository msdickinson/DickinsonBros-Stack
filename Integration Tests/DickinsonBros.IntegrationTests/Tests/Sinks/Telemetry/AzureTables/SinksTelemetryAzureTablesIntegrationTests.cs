using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
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
        private readonly ITelemetryWriterService _telemetryWriterService;
        private readonly ICorrelationService _correlationService;
        private readonly ISinksTelemetryAzureTablesService<StorageAccountDickinsonBros> _sinksTelemetryAzureTablesService;
        public SinksTelemetryAzureTablesIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService,
            ICorrelationService correlationService,
            ISinksTelemetryAzureTablesService<StorageAccountDickinsonBros> sinksTelemetryAzureTablesService
        )
        {
            _telemetryWriterService = telemetryWriterService;
            _correlationService = correlationService;
            _sinksTelemetryAzureTablesService = sinksTelemetryAzureTablesService;
        }

        public async Task InsertAndFlush_Runs_DoesNotThrow(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "SinksTelemetryAzureTables";

            var insertTelemetryItem = new InsertTelemetryItem()
            {
                DateTimeUTC = DateTime.UtcNow,
                ConnectionName = "SampleConnectionName",
                Duration = TimeSpan.FromSeconds(1),
                Request = "SampleSignalRequest",
                CorrelationId = _correlationService.CorrelationId,
                TelemetryResponseState = TelemetryResponseState.Successful,
                TelemetryType = TelemetryType.Application,
            };

            _telemetryWriterService.Insert(insertTelemetryItem);

            await _sinksTelemetryAzureTablesService.FlushAsync().ConfigureAwait(false);
        }

    }
}
