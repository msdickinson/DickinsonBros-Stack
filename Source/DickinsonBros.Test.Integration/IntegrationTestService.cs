using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration
{
    public class IntegrationTestService : IIntegrationTestService
    {
        internal const string NULL_TEST_CLASS_ERROR_MESSAGE = "TestClass Is Null, Please ensure when calling SetupTests that the input has a value";
        internal const string SUCCESSLOG_PRAM_NAME = "successLog";
        internal const string CORRELATIONID_EXPECTED_PRAM_NAME = "correlationId";

        internal readonly ICorrelationService _correlationService;
        internal readonly ITRXReportService _trxReportService;
        internal readonly IGuidService _guidService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly IStopwatchFactory _stopwatchFactory;


        public delegate void NewTelemetryEventHandler(TestSummary testSummary);
        event IIntegrationTestService.NewTestSummaryEventHandler NewTestSummaryEventHandler;
        public IntegrationTestService
        (
            ITRXReportService trxReportService,
            ICorrelationService correlationService,
            IGuidService guidService,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory
        )
        {
            _trxReportService = trxReportService;
            _correlationService = correlationService;
            _guidService = guidService;
            _dateTimeService = dateTimeService;
            _stopwatchFactory = stopwatchFactory;
        }

        [ExcludeFromCodeCoverage]
        event IIntegrationTestService.NewTestSummaryEventHandler IIntegrationTestService.NewTestSummaryEvent
        {
            add => NewTestSummaryEventHandler += value;
            remove => NewTestSummaryEventHandler -= value;
        }

        public IEnumerable<Abstractions.Models.Test> SetupTests(object testClass)
        {
            if (testClass == null)
            {
                throw (new NullReferenceException(NULL_TEST_CLASS_ERROR_MESSAGE));
            }

            var tests = new List<Abstractions.Models.Test>();

            var testAPIAttribute = (TestAPIAttribute)System.Attribute.GetCustomAttributes(testClass.GetType()).FirstOrDefault(e => e is TestAPIAttribute);

            var methodInfos = testClass
                 .GetType()
                 .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                 .ToList();

            foreach (Type interf in testClass.GetType().GetInterfaces())
            {
                foreach (MethodInfo method in interf.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!methodInfos.Any(e => e.Name == method.Name))
                    {
                        methodInfos.Add(method);
                    }
                }
            }

            var filteredList = methodInfos
                .Where
                (
                    methodInfo =>
                    methodInfo.GetParameters().Length == 1 &&
                    methodInfo.GetParameters()[0].Name == SUCCESSLOG_PRAM_NAME &&
                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                    methodInfo.ReturnType == typeof(Task)
                )
                .ToList();

            return filteredList.Select(method => new Abstractions.Models.Test
            {
                MethodInfo = method,
                TestClass = testClass,
                TestsName = testAPIAttribute?.Name,
                TestGroup = testAPIAttribute?.Group,
            }).AsEnumerable();
        }

        public async Task<TestSummary> RunTests(IEnumerable<Abstractions.Models.Test> tests)
        {
            var testSummary = new TestSummary();
            var tasks = new List<Task<TestResult>>();

            foreach (var test in tests)
            {
                tasks.Add(Process(test.TestClass, test));
            };

            testSummary.StartDateTime = _dateTimeService.GetDateTimeUTC();
            testSummary.TestResults = (await Task.WhenAll(tasks)).ToList();
            testSummary.EndDateTime = _dateTimeService.GetDateTimeUTC();
            testSummary.Id = _guidService.NewGuid().ToString();
            return testSummary;
        }

        internal async Task<TestResult> Process<T>(T tests, Abstractions.Models.Test test)
        {
            _correlationService.CorrelationId = _guidService.NewGuid().ToString();
            bool pass = false;
            Exception exception = null;
            var successLog = new List<string>();

            var startDateTime = _dateTimeService.GetDateTimeUTC();

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                await (Task)test.MethodInfo.Invoke(tests, new object[] { successLog });
                pass = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                stopwatchService.Stop();
            }
            var endDateTime = _dateTimeService.GetDateTimeUTC();
            return new TestResult
            {
                ClassName = test.MethodInfo.ReflectedType.Name,
                TestsName = test.TestsName ?? test.MethodInfo.ReflectedType.Name,
                TestName = test.MethodInfo.Name,
                TestGroup = test.TestGroup,
                CorrelationId = _correlationService.CorrelationId,
                Pass = pass,
                Duration = stopwatchService.Elapsed,
                Exception = exception,
                SuccessLog = successLog,
                StartTime = startDateTime,
                EndTime = endDateTime,
                ExecutionId = _guidService.NewGuid(),
                TestId = _guidService.NewGuid(),
                TestType = _guidService.NewGuid()

            };
        }

        public string GenerateLog(TestSummary testSummary, bool showSuccessLogsOnSuccess)
        {
            var logs = new List<string>();

            foreach (var testGroup in testSummary.TestResults.GroupBy(e => e.TestsName))
            {
                logs.Add(testGroup.Key);
                logs.Add("");
                foreach (var result in testGroup.OrderBy(e => e.TestName))
                {
                    var pass = result.Pass ? "PASS" : "FAIL";
                    logs.Add($"{ pass }  { result.TestName } - { result.CorrelationId }");
                    if (
                        result.SuccessLog.Any() &&
                        (result.Pass && showSuccessLogsOnSuccess) ||
                        (!result.Pass)
                      )
                    {
                        foreach (var log in result.SuccessLog)
                        {
                            logs.Add($"+ {log}");
                        }
                    }
                    if (!result.Pass)
                    {
                        logs.Add($"- {result.Exception.Message}");
                        logs.Add("");
                    }
                    if (
                           result.SuccessLog.Any() &&
                           result.Pass &&
                           showSuccessLogsOnSuccess
                       )
                    {
                        logs.Add("");
                    }
                }
                logs.Add("");
            }

            return String.Join(Environment.NewLine, logs.ToArray());
        }
 
        public string GenerateTRXReport(TestSummary testSummary)
        {
            return _trxReportService.GenerateTRXReport(testSummary);
        }

        public void TestSummaryPublishEvent(TestSummary testSummary)
        {
            NewTestSummaryEventHandler.Invoke(testSummary);
        }
    }
}
