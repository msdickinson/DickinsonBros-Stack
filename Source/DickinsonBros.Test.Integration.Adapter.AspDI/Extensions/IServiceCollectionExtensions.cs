using DickinsonBros.Test.Integration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Test.Integration.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationTestService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IIntegrationTestService, IntegrationTestService>();
            serviceCollection.TryAddSingleton<ITRXReportService, TRXReportService>();
            return serviceCollection;
        }
    }
}
