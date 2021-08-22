using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Middleware.Function.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class ProcessRequestDescriptor<T> 
    where T : class
    {
        public bool IsSuccessful { get; set; }
        public ContentResult ContentResult { get; set; }
        public T Data { get; set; }
    }
}
