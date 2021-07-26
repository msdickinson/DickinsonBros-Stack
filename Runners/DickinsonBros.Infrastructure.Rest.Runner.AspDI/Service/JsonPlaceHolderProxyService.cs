using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy.Models;
using DickinsonBros.Infrastructure.Rest.Abstractions;
using DickinsonBros.Infrastructure.Rest.Abstractions.Models;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Models.Models;

namespace DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy
{
    public class JsonPlaceHolderProxyService : IJsonPlaceHolderProxyService
    {
        internal readonly JsonPlaceHolderProxyOptions _options;
        internal readonly IRestService _restService;
        internal readonly HttpClient _httpClient;

        public JsonPlaceHolderProxyService(IRestService restService, HttpClient httpClient, IOptions<JsonPlaceHolderProxyOptions> options)
        {
            _restService = restService;
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<HttpResponse<Todo>> GetTodosAsync(GetTodosRequest getTodosRequest)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_options.GetTodosResource}{getTodosRequest.Items}", UriKind.Relative)
            };

            return await _restService.ExecuteAsync<Todo>("https://jsonplaceholder.typicode.com/todos/", _httpClient, httpRequestMessage, _options.GetTodosRetrys, _options.GetTodosTimeoutInSeconds).ConfigureAwait(false);
        }
    }
}
