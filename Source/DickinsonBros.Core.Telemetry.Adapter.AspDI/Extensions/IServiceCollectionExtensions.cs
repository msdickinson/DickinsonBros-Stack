using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Configurators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddTelemetryWriterService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ITelemetryWriterService, TelemetryWriterService>();
            serviceCollection.TryAddSingleton<IConfigureOptions<TelemetryWriterServiceOptions>, TelemetryWriterServiceOptionsConfigurator>();

            return serviceCollection;
        }
    }
}
