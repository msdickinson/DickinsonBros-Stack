using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Guid
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Guid", Group = "Core")]
    public class GuidIntegrationTests : IGuidIntegrationTests
    {
        public IGuidService _guidService;

        public GuidIntegrationTests
        (
            IGuidService guidService
        )
        {
            _guidService = guidService;
        }
        public async Task NewGuid_Runs_AValueGuid(List<string> successLog)
        {
            var guid = _guidService.NewGuid();
            Assert.IsNotNull(guid, "Guid is null");
            successLog.Add($"Guid: {guid}");

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
