using DickinsonBros.Core.Logger.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DickinsonBros.Core.Logger.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggerService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

            return serviceCollection;
        }
    }
}
