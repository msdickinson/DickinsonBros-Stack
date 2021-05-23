using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions
{
    public static class IServiceColflectionExtensions
    {
        public static IServiceCollection AddSinksTelemetryLogServiceService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISinksTelemetryLogService, SinksTelemetryLogService>();
            return serviceCollection;
        }
    }
}
