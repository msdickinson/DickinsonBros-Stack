using DickinsonBros.Infrastructure.Rest.Abstractions.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Rest.Abstractions
{
    public interface IRestService
    {
        Task<HttpResponseMessage> ExecuteAsync(string connectionName, HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds);
        Task<HttpResponse<T>> ExecuteAsync<T>(string connectionName, HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds);

    }
}
