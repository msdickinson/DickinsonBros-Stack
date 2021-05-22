using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Integration.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration.Tests
{
    [TestClass]
    public class TestAutomationTests : BaseTest
    {
        #region FetchTests

        [TestMethod]
        public async Task FetchTests_Runs_ReturnsTests()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.FetchTests();

                    //Assert
                    Assert.AreEqual(6, observed.Count());
                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchTestsByGroup

        [TestMethod]
        public async Task FetchTestsByGroup_ByTestName_ReturnsTests()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var groupname = "TestGroup";

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.FetchTestsByGroup(groupname);

                    //Assert
                    Assert.AreEqual(4, observed.Count());
                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchTestsByName

        [TestMethod]
        public async Task FetchTestsByName_ByTestName_ReturnsTests()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var name = "MixedTests";

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.FetchTestsByName(name);

                    //Assert
                    Assert.AreEqual(4, observed.Count());
                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchTestsByName

        [TestMethod]
        public async Task FetchTestsByTestName_ByTestName_ReturnsTests()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var testName = "Example_MethodOne_Async";

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    //Act
                    var observed = uutConcrete.FetchTestsByTestName(testName);

                    //Assert
                    Assert.AreEqual(1, observed.Count());
                    await Task.CompletedTask.ConfigureAwait(false);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region RunTests

        [TestMethod]
        public async Task RunTests_MethodRuns_ReturnsTestResults()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange​
                    var exampleTestClass = new MixedClass();

                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(e => e.DeclaringType.Name == exampleTestClass.GetType().Name);

                    var tests = new List<Abstractions.Models.Test>
                    {
                        new Abstractions.Models.Test()
                        {
                            MethodInfo = expectedMethodInfos.First(),
                            TestClass = exampleTestClass
                        },
                        new Abstractions.Models.Test()
                        {
                            MethodInfo = expectedMethodInfos.Last(),
                            TestClass = exampleTestClass
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.RunTests(tests).ConfigureAwait(false);

                    // Assert​
                    Assert.IsNotNull(observed);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndDateTime);
                    Assert.IsNotNull(observed.StartDateTime);
                    Assert.AreEqual(2, observed.TestResults.Count());
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Process

        [TestMethod]
        public async Task Process_MethodRuns_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleTestClass = new ExampleTestClass();

                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where
                                           (
                                                methodInfo =>
                                                    methodInfo.GetParameters().Length == 1 &&
                                                    methodInfo.GetParameters()[0].Name == IntegrationTestService.SUCCESSLOG_PRAM_NAME &&
                                                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                                                    methodInfo.ReturnType == typeof(Task)
                                           );

                    var test = new Abstractions.Models.Test()
                    {
                        MethodInfo = expectedMethodInfos.First(),
                        TestClass = exampleTestClass
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.IsNull(observed.TestGroup);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsTrue(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(1, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task Process_MethodRunsWithTestClassAndTestGroup_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleMixedClass = new MixedClass();

                    var expectedMethodInfos = exampleMixedClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where
                                           (
                                                methodInfo =>
                                                    methodInfo.GetParameters().Length == 1 &&
                                                    methodInfo.GetParameters()[0].Name == IntegrationTestService.SUCCESSLOG_PRAM_NAME &&
                                                    methodInfo.GetParameters()[0].ParameterType == typeof(List<string>) &&
                                                    methodInfo.ReturnType == typeof(Task)
                                           );
                    var testAPIAttribute = (TestAPIAttribute)System.Attribute.GetCustomAttributes(exampleMixedClass.GetType()).FirstOrDefault(e => e is TestAPIAttribute);

                    var test = new Abstractions.Models.Test()
                    {
                        MethodInfo = expectedMethodInfos.First(),
                        TestClass = exampleMixedClass,
                        TestGroup = testAPIAttribute.Group,
                        TestsName = testAPIAttribute.Name
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.AreEqual(test.TestGroup, observed.TestGroup);
                    Assert.AreEqual(test.TestsName, observed.TestsName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsTrue(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(0, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        public async Task Process_MethodThrows_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var exampleTestClass = new ExampleTestClass();

                    var expectedMethodInfos = exampleTestClass
                                           .GetType()
                                           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(e => e.DeclaringType.Name == exampleTestClass.GetType().Name);

                    var test = new Abstractions.Models.Test()
                    {
                        MethodInfo = expectedMethodInfos.Last(),
                        TestClass = exampleTestClass
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = await uutConcrete.Process(test.TestClass, test).ConfigureAwait(false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(test.MethodInfo.ReflectedType.Name, observed.ClassName);
                    Assert.IsNotNull(observed.CorrelationId);
                    Assert.IsNotNull(observed.Duration);
                    Assert.IsNotNull(observed.EndTime);
                    Assert.IsNotNull(observed.Exception);
                    Assert.IsNotNull(observed.ExecutionId);
                    Assert.IsFalse(observed.Pass);
                    Assert.IsNotNull(observed.StartTime);
                    Assert.AreEqual(1, observed.SuccessLog.Count());
                    Assert.IsNotNull(observed.TestId);
                    Assert.IsNotNull(test.MethodInfo.Name, observed.TestName);
                    Assert.IsNotNull(observed.TestType);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GenerateTRXReport

        [TestMethod]
        public async Task GenerateTRXReport_Runs_TrxReportServiceGenerateTRXReportCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange
                    var trxReportServiceMock = serviceProvider.GetMock<ITRXReportService>();
                    var expectedSummary = "";
                    trxReportServiceMock
                    .Setup
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            It.IsAny<TestSummary>()
                        )
                    )
                    .Returns
                    (
                        expectedSummary
                    );

                    var testSummary = new TestSummary();

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateTRXReport(testSummary);

                    // Assert
                    trxReportServiceMock
                    .Verify
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            testSummary
                        ),
                        Times.Once
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task GenerateTRXReport_Runs_ReturnsStrinFromTrxReportServiceGenerateResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange
                    var trxReportServiceMock = serviceProvider.GetMock<ITRXReportService>();
                    var expectedSummary = "";
                    trxReportServiceMock
                    .Setup
                    (
                        trxReportService => trxReportService.GenerateTRXReport
                        (
                            It.IsAny<TestSummary>()
                        )
                    )
                    .Returns
                    (
                        expectedSummary
                    );

                    var testSummary = new TestSummary();

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateTRXReport(testSummary);

                    // Assert
                    Assert.AreEqual(expectedSummary, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region GenerateLog

        [TestMethod]
        public async Task GenerateLog_ShowSuccessLogsOnSuccessFalse_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new TestSummary
                    {
                        Duration = TimeSpan.FromMinutes(1),
                        StartDateTime = System.DateTime.Now.AddMinutes(-1),
                        EndDateTime = System.DateTime.Now,
                        TestResults = new List<Abstractions.Models.TestResult>
                        {
                            new Abstractions.Models.TestResult
                            {
                                TestGroup = "SampleTestGroup",
                                TestsName = "SampleTestsName",
                                ClassName = "SampleClassname",
                                CorrelationId = "SampleCorrelationId",
                                Duration = TimeSpan.FromMinutes(1),
                                StartTime = System.DateTime.Now,
                                EndTime = System.DateTime.Now.AddMinutes(-1),
                                Exception = null,
                                ExecutionId = System.Guid.NewGuid(),
                                Pass = true,
                                SuccessLog = new List<string>{ "Part 1 Successful" },
                                TestId = System.Guid.NewGuid(),
                                TestName = "SampleTestName1",
                                TestType = System.Guid.NewGuid()
                            },
                            new Abstractions.Models.TestResult
                            {
                                TestGroup = "SampleTestGroup",
                                TestsName = "SampleTestsName",
                                ClassName = "SampleClassname",
                                CorrelationId = "SampleCorrelationId",
                                Duration = TimeSpan.FromMinutes(1),
                                StartTime = System.DateTime.Now,
                                EndTime = System.DateTime.Now.AddMinutes(-1),
                                Exception = new Exception("SampleException"),
                                ExecutionId = System.Guid.NewGuid(),
                                Pass = false,
                                SuccessLog = new List<string>{ "Part 1 Successful" },
                                TestId = System.Guid.NewGuid(),
                                TestName = "SampleTestName2",
                                TestType = System.Guid.NewGuid()
                            }
                        }
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateLog(testSummary, false);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual("SampleTestsName\r\n\r\nPASS  SampleTestName1 - SampleCorrelationId\r\nFAIL  SampleTestName2 - SampleCorrelationId\r\n+ Part 1 Successful\r\n- SampleException\r\n\r\n\r\n1 tests successful\r\n1 tests failed:", observed);
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task GenerateLog_ShowSuccessLogsOnSuccessTrue_ReturnsTestResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new TestSummary
                    {
                        Duration = TimeSpan.FromMinutes(1),
                        StartDateTime = System.DateTime.Now.AddMinutes(-1),
                        EndDateTime = System.DateTime.Now,
                        TestResults = new List<Abstractions.Models.TestResult>
                    {
                        new Abstractions.Models.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = System.DateTime.Now,
                            EndTime = System.DateTime.Now.AddMinutes(-1),
                            Exception = null,
                            ExecutionId = System.Guid.NewGuid(),
                            Pass = true,
                            SuccessLog = new List<string> { "Part 1 Successful" },
                            TestId = System.Guid.NewGuid(),
                            TestName = "SampleTestName1",
                            TestType = System.Guid.NewGuid()
                        },
                        new Abstractions.Models.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = System.DateTime.Now,
                            EndTime = System.DateTime.Now.AddMinutes(-1),
                            Exception = null,
                            ExecutionId = System.Guid.NewGuid(),
                            Pass = true,
                            SuccessLog = new List<string> { },
                            TestId = System.Guid.NewGuid(),
                            TestName = "SampleTestName2",
                            TestType = System.Guid.NewGuid()
                        },
                        new Abstractions.Models.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = System.DateTime.Now,
                            EndTime = System.DateTime.Now.AddMinutes(-1),
                            Exception = new Exception("SampleException"),
                            ExecutionId = System.Guid.NewGuid(),
                            Pass = false,
                            SuccessLog = new List<string> { "Part 1 Successful" },
                            TestId = System.Guid.NewGuid(),
                            TestName = "SampleTestName3",
                            TestType = System.Guid.NewGuid()
                        },
                        new Abstractions.Models.TestResult
                        {
                            TestGroup = "SampleTestGroup",
                            TestsName = "SampleTestsName",
                            ClassName = "SampleClassname",
                            CorrelationId = "SampleCorrelationId",
                            Duration = TimeSpan.FromMinutes(1),
                            StartTime = System.DateTime.Now,
                            EndTime = System.DateTime.Now.AddMinutes(-1),
                            Exception = new Exception("SampleException"),
                            ExecutionId = System.Guid.NewGuid(),
                            Pass = false,
                            SuccessLog = new List<string> { },
                            TestId = System.Guid.NewGuid(),
                            TestName = "SampleTestName4",
                            TestType = System.Guid.NewGuid()
                        }

                    }
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    // Act

                    var observed = uutConcrete.GenerateLog(testSummary, true);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual("SampleTestsName\r\n\r\nPASS  SampleTestName1 - SampleCorrelationId\r\n+ Part 1 Successful\r\n\r\nPASS  SampleTestName2 - SampleCorrelationId\r\nFAIL  SampleTestName3 - SampleCorrelationId\r\n+ Part 1 Successful\r\n- SampleException\r\n\r\nFAIL  SampleTestName4 - SampleCorrelationId\r\n- SampleException\r\n\r\n\r\n2 tests successful\r\n2 tests failed:", observed);
                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region TestSummaryPublishEvent

        [TestMethod]
        public async Task TestSummaryPublishEvent_EventPublished_RetriveEvent()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new TestSummary
                    {
                   
                    };

                    var uut = serviceProvider.GetRequiredService<IIntegrationTestService>();
                    var uutConcrete = (IntegrationTestService)uut;

                    var eventDataObserved = (TestSummary)null;
                    uut.NewTestSummaryEvent += (testSummary) =>
                    {
                        eventDataObserved = testSummary;
                    };

                    // Act

                    uutConcrete.TestSummaryPublishEvent(testSummary);

                    // Assert

                    Assert.IsNotNull(eventDataObserved);
                    Assert.AreEqual(testSummary, eventDataObserved);
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }




        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIntegrationTestService, IntegrationTestService>();
            serviceCollection.AddSingleton<IExampleTestClass, ExampleTestClass>();
            serviceCollection.AddSingleton<InterfaceOne, MixedClass>();
            serviceCollection.AddSingleton(Mock.Of<ITRXReportService>());
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            
            serviceCollection.AddStopwatchFactory();
            return serviceCollection;
        }

        public interface IExampleTestClass : ITestsInterface
        {
            Task Example_MethodOne_Async(List<string> successLog);
            Task Example_MethodTwo_Async(List<string> successLog);
        }


        public class ExampleTestClass : IExampleTestClass
        {
            public async Task Example_MethodOne_Async(List<string> successLog)
            {
                successLog.Add("Step 1 Successful");
                await Task.CompletedTask.ConfigureAwait(false);
            }
            public async Task Example_MethodTwo_Async(List<string> successLog)
            {
                successLog.Add("Step 1 Successful");
                await Task.CompletedTask.ConfigureAwait(false);
                throw new Exception("SampleException");
            }
        }


        [TestAPIAttribute(Name = "MixedTests", Group = "TestGroup")]
        public class MixedClass : InterfaceOne, InterfaceTwo
        {
            internal async Task MethodOne_NotMatch() { await Task.CompletedTask.ConfigureAwait(false); }
            private async Task MethodTwo_NotMatch() { await Task.CompletedTask.ConfigureAwait(false); }
            public void MethodThree_NotMatch() { }
            public void MethodFour_NotMatch(List<string> successLog) { }
            public async Task MethodFive_Match(List<string> successLog) { await Task.CompletedTask.ConfigureAwait(false); }

            public async Task MethodSix_Matchs(List<string> successLog)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }

            async Task InterfaceTwo.MethodSeven_Matchs(List<string> successLog)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }

            async Task InterfaceThree.MethodEight_Matchs(List<string> successLog)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        public interface InterfaceOne : ITestsInterface
        {
            Task MethodSix_Matchs(List<string> successLog);
        }

        public interface InterfaceTwo : InterfaceThree
        {
            Task MethodSeven_Matchs(List<string> successLog);
        }
        public interface InterfaceThree
        {
            Task MethodEight_Matchs(List<string> successLog);
        }
        #endregion
    }
}
