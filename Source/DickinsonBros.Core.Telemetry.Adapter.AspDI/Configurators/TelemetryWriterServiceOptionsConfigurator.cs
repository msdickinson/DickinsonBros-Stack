using DickinsonBros.Core.Telemetry.Abstractions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Core.Telemetry.Adapter.AspDI.Configurators
{
    public class TelemetryWriterServiceOptionsConfigurator : IConfigureOptions<TelemetryWriterServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TelemetryWriterServiceOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<TelemetryWriterServiceOptions>.Configure(TelemetryWriterServiceOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(TelemetryWriterServiceOptions)}";
            configuration.Bind(path, options);
        }
    }
}
