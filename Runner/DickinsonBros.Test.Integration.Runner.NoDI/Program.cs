using Dickinsonbros.Core.Guid;
using DickinsonBros.Core.Correlation;
using DickinsonBros.Core.DateTime;
using DickinsonBros.Core.Stopwatch;
using DickinsonBros.Test.Integration.Runner.NoDI.ExampleTest;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var correlationService      = new CorrelationService();
            var guidService             = new GuidService();
            var dateTimeService         = new DateTimeService();
            var stopwatchFactory        = new StopwatchFactory();
            var trxReportService        = new TRXReportService(guidService);
            var integrationTestService  = 
                new IntegrationTestService
            (
                trxReportService,
                correlationService,
                guidService,
                dateTimeService,
                stopwatchFactory
            );

            //Setup Tests
            var tests = integrationTestService.SetupTests();

            //Run Tests
            var testSummary = await integrationTestService.RunTests(tests).ConfigureAwait(false);

            //Process Test Summary
            var trxReport = integrationTestService.GenerateTRXReport(testSummary);
            var log = integrationTestService.GenerateLog(testSummary, true);

            //Console Summary
            Console.WriteLine("Log:");
            Console.WriteLine(log);
            Console.WriteLine();

            Console.WriteLine("TRX Report:");
            Console.WriteLine(trxReport);
            Console.WriteLine();
        }
    }
}