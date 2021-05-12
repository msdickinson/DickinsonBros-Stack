using DickinsonBros.Test.Integration.Abstractions.Models;

namespace DickinsonBros.Test.Integration.Abstractions
{
    public interface ITRXReportService
    {
        string GenerateTRXReport(TestSummary testSummary);
    }
}
