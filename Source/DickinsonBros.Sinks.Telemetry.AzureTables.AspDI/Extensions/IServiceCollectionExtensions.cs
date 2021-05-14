using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions
{
    public static class IServiceColflectionExtensions
    {
        public static IServiceCollection AddSinksTelemetryAzureTablesService<T>(this IServiceCollection serviceCollection)
        where T : AzureTableServiceOptionsType
        {
            serviceCollection.TryAddSingleton<ITelemetryServiceWriter, SinksTelemetryAzureTablesService<T>>();
            return serviceCollection;
        }
    }
}
