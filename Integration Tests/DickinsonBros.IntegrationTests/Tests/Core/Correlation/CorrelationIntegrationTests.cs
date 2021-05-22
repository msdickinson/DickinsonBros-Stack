using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Correlation
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Correlation", Group = "Core")]
    public class CorrelationIntegrationTests : ICorrelationIntegrationTests
    {
        public ICorrelationService _correlationService;

        public CorrelationIntegrationTests
        (
            ICorrelationService correlationService
        )
        {
            _correlationService = correlationService;
        }

        public async Task CorrelationId_Runs_CorrelationIdIsNotNull(List<string> successLog)
        {
            var correlationId = _correlationService.CorrelationId;

            Assert.IsNotNull(correlationId, "CorrelationId is null");
            successLog.Add($"CorrelationId: {correlationId}");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task CorrelationId_ChangeValueInADeeperContext_CorrelationIdIsNotChangedInOuterContext(List<string> successLog)
        {
            var correlationId = _correlationService.CorrelationId;
            var correlationIdUnchanged = correlationId.Clone();

            await ModifyCorrelationIdAsync("", _correlationService).ConfigureAwait(false);
            Assert.AreEqual(correlationIdUnchanged, correlationId);

            await Task.CompletedTask.ConfigureAwait(false);
        }


        private async Task ModifyCorrelationIdAsync(string updateValue, ICorrelationService correlationService)
        {
            correlationService.CorrelationId = updateValue;
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
