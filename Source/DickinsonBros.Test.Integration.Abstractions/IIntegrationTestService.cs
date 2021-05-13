using DickinsonBros.Test.Integration.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration.Abstractions
{
    public interface IIntegrationTestService
    {
        IEnumerable<Models.Test> FetchTests();
        IEnumerable<Models.Test> FetchTestsByGroup(string group);
        IEnumerable<Models.Test> FetchTestsByName(string name);
        IEnumerable<Models.Test> FetchTestsByTestName(string testName);
        Task<TestSummary> RunTests(IEnumerable<Models.Test> tests);
        string GenerateLog(TestSummary testSummary, bool showSuccessLogsOnSuccess);
        string GenerateTRXReport(TestSummary testSummary);
        event NewTestSummaryEventHandler NewTestSummaryEvent;
        void TestSummaryPublishEvent(TestSummary testSummary);
        public delegate void NewTestSummaryEventHandler(TestSummary testSummary);
    }
}
