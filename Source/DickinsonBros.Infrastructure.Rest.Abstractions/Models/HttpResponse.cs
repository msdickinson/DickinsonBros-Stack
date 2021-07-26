using System.Net.Http;

namespace DickinsonBros.Infrastructure.Rest.Abstractions.Models
{
    public class HttpResponse<T>
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public T Data { get; set; }
    }
}
