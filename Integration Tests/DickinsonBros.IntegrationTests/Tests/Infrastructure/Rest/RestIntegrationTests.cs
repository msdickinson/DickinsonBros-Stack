using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Rest
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Rest", Group = "Infrastructure")]
    public class RestIntegrationTests : IRestIntegrationTests
    {
        public readonly IRestService _restService;

        public readonly ITelemetryWriterService _telemetryWriterService;
        public RestIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService,
            IRestService restService
        )
        {
            _telemetryWriterService = telemetryWriterService;
            _restService = restService;
        }

        public async Task GetTodos_Runs_ExpectedResponse(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "Rest";

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("todos/1", UriKind.Relative)
            };

            var retrys = 3;
            var timeoutInSeconds = 30;

            var restResponse = await _restService.ExecuteAsync<Todo>("https://jsonplaceholder.typicode.com/todos/", httpClient, httpRequestMessage, retrys, timeoutInSeconds).ConfigureAwait(false);
          
            Assert.AreEqual(HttpStatusCode.OK, restResponse.HttpResponseMessage.StatusCode, $"Expected Successful. Status Code: {restResponse.HttpResponseMessage.StatusCode}");
            successLog.Add($"Status Code Successful"); 

            Assert.IsNotNull(restResponse.Data, $"Data is null");
            successLog.Add($"Data is not null");
        }
    }
}
