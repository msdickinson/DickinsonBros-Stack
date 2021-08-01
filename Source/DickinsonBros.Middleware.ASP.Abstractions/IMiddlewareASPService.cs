using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DickinsonBros.MiddlewareService.ASP.Abstractions
{
    public interface IMiddlewareASPService
    {
        Task InvokeAsync(HttpContext context);
    }
}
