using DickinsonBros.Middleware.Function.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DickinsonBros.Middleware.Function.Abstractions
{
    public interface IFunctionHelperService
    {
        Task<ProcessRequestDescriptor<T>> ProcessRequestAsync<T>(HttpRequest httpRequest) where T : class;
        ContentResult StatusCode(int statusCode);
        ContentResult StatusCode(int statusCode, string text);
        ContentResult StatusCode<T>(int statusCode, T data) where T : class;
    }
}
