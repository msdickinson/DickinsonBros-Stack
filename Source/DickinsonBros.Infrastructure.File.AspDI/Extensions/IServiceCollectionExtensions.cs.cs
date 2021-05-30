using DickinsonBros.Infrastructure.File.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Infrastructure.File.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFileService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IFileService, FileService>();
            return serviceCollection;
        }
    }
}
