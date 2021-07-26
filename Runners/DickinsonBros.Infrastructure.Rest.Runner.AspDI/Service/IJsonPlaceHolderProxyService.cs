using DickinsonBros.Infrastructure.Rest.Abstractions.Models;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Models.Models;
using DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy
{
    public interface IJsonPlaceHolderProxyService
    {
        Task<HttpResponse<Todo>> GetTodosAsync(GetTodosRequest getTodosRequest);
    }
}