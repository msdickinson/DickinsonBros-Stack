using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Test.Integration.Abstractions;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace DickinsonBros.Test.Integration.Tests
{
    [TestClass]
    public class TRXReportServiceTests : BaseTest
    {
        #region GenerateTRXReport

        [TestMethod]
        public async Task GenerateTRXReport_Runs_ExpectedStringIsReturns()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    var testSummary = new Abstractions.Models.TestSummary
                    {
                        Duration = TimeSpan.FromMinutes(1),
                        StartDateTime = System.DateTime.Now.AddMinutes(-1),
                        EndDateTime = System.DateTime.Now,
                        TestResults = new List<Abstractions.Models.TestResult>
                        {
                            new Abstractions.Models.TestResult
                            {
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
                                TestName = "SampleTestName",
                                TestType = System.Guid.NewGuid()
                            },
                            new Abstractions.Models.TestResult
                            {
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
                                TestName = "SampleTestName",
                                TestType = System.Guid.NewGuid()
                            }

                        }
                    };


                    var uut = serviceProvider.GetRequiredService<ITRXReportService>();
                    var uutConcrete = (TRXReportService)uut;

                    // Act

                    var observed = uutConcrete.GenerateTRXReport(testSummary);

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.IsTrue(observed.Contains("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));
                    var document = new XmlDocument();
                    document.LoadXml(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITRXReportService, TRXReportService>();
            serviceCollection.AddSingleton(Mock.Of<IGuidService>());
            return serviceCollection;
        }

        #endregion
    }
}
