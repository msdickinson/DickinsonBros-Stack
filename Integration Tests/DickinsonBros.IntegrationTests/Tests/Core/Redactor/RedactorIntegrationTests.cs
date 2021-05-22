using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Logger
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Redactor", Group = "Core")]
    public class RedactorIntegrationTests : IRedactorIntegrationTests
    {
        public class SampleContract
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public IRedactorService _redactorService;

        public RedactorIntegrationTests
        (
            IRedactorService redactorService
        )
        {
            _redactorService = redactorService;
        }

        public async Task Redact_JsonString_ReturnsRedactedString(List<string> successLog)
        {
            var input =
@"{
  ""Password"": ""password""
}";

            var expectedRedactedOutput = "{\r\n  \"Password\": \"***REDACTED***\"\r\n}";

            var output = _redactorService.Redact(input);

            Assert.AreEqual(expectedRedactedOutput, output, "Does not match the expected output");
            successLog.Add($"Output is redacted as expected. \r\nInput: \r\n{input} \r\nOutput: \r\n{output}");
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task Redact_string_ReturnsRedactedString(List<string> successLog)
        {
            var input = @"password";

            var expectedRedactedOutput = "***REDACTED***";

            var output = _redactorService.Redact(input);

            Assert.AreEqual(expectedRedactedOutput, output, "Does not match the expected output");
            successLog.Add($"Output is redacted as expected. \r\nInput: \r\n{input} \r\nOutput: \r\n{output}");
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task Redact_Object_ReturnsRedactedString(List<string> successLog)
        {
            //Run application
            var sampleContract = new SampleContract
            {
                Password = "SamplePassword",
                Username = "SampleUsername"
            };

            var input = System.Text.Json.JsonSerializer.Serialize(sampleContract);

            var expectedRedactedOutput = "{\r\n  \"Username\": \"SampleUsername\",\r\n  \"Password\": \"***REDACTED***\"\r\n}";

            var output = _redactorService.Redact(sampleContract);

            Assert.AreEqual(expectedRedactedOutput, output, "Does not match the expected output");
            successLog.Add($"Output is redacted as expected. \r\nInput: \r\n{input} \r\nOutput: \r\n{output}");
            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
