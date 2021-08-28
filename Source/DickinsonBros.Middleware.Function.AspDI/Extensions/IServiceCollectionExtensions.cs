using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.JWT.Abstractions.Models;
using DickinsonBros.Middleware.Function;
using DickinsonBros.Middleware.Function.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dickinsonbros.Middleware.Function.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMiddlwareFunctionService<T, U>(this IServiceCollection serviceCollection, IConfiguration configuration)
        where T : JWTEncryptionServiceOptionsType
        where U : CertificateEncryptionServiceOptionsType
        {
            serviceCollection.TryAddSingleton(typeof(IMiddlewareFunctionService), typeof(MiddlewareFunctionService<T, U>));
            serviceCollection.TryAddSingleton<IFunctionHelperService, FunctionHelperService>();

            return serviceCollection;
        }
    }
}