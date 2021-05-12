using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Guid
{
    public interface IGuidIntegrationTests
    {
        Task NewGuid_Runs_AValueGuid(List<string> successLog);
    }
}