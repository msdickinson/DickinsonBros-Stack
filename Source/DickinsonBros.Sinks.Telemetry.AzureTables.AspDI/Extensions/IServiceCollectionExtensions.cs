using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Configurators;
using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions
{
    public static class IServiceColflectionExtensions
    {
        public static IServiceCollection AddSinksTelemetryAzureTablesService<T>(this IServiceCollection serviceCollection)
        where T : AzureTableServiceOptionsType
        {
            serviceCollection.TryAddSingleton<ISinksTelemetryAzureTablesService<T>, SinksTelemetryAzureTablesService<T>>();
            serviceCollection.AddSingleton<IConfigureOptions<SinksTelemetryAzureTablesServiceOptions>, SinksTelemetryAzureTablesServiceConfigurator>();
            return serviceCollection;
        }
    }
}
