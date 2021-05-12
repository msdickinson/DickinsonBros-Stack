using DickinsonBros.IntegrationTests.Tests.Core.DateTime;
using DickinsonBros.IntegrationTests.Tests.Core.Guid;
using DickinsonBros.Test.Integration.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
{
    public class Runner : IRunner
    {
        private readonly IIntegrationTestService _integrationTestService;

        private readonly IGuidIntegrationTests _guidIntegrationTests;
        private readonly IDateTimeIntegrationTests _dateTimeIntegrationTests;


        public Runner
        (
            IIntegrationTestService integrationTestService,
            IGuidIntegrationTests guidIntegrationTests,
            IDateTimeIntegrationTests dateTimeIntegrationTests

        )
        {
            _integrationTestService = integrationTestService;
            _guidIntegrationTests = guidIntegrationTests;
            _dateTimeIntegrationTests = dateTimeIntegrationTests;
        }

        public async Task<string> RunAsync()
        {
            //Setup Tests
            var tests = new List<Test.Integration.Abstractions.Models.Test>();
            _integrationTestService.SetupTests
            (
                new 
                { 
                    _guidIntegrationTests,
                    _dateTimeIntegrationTests
                }
            );

            //Filter Tests

            //Run Tests
            var testSummary = await _integrationTestService.RunTests(tests).ConfigureAwait(false);

            //Process Test Summary
            return _integrationTestService.GenerateLog(testSummary, true);
        }

    }
}
