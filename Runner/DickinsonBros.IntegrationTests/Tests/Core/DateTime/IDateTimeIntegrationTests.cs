using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.DateTime
{
    public interface IDateTimeIntegrationTests
    {
        Task GetDateTimeUTC_Runs_DateIsNotNull(List<string> successLog);
    }
}