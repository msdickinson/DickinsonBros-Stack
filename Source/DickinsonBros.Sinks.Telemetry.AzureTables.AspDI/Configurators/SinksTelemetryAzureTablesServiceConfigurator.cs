using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Configurators
{
    public class SinksTelemetryAzureTablesServiceConfigurator : IConfigureOptions<SinksTelemetryAzureTablesServiceOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public SinksTelemetryAzureTablesServiceConfigurator(IServiceScopeFactory serviceScopeFactory)
        { 
            _serviceScopeFactory = serviceScopeFactory;
        }
        void IConfigureOptions<SinksTelemetryAzureTablesServiceOptions>.Configure(SinksTelemetryAzureTablesServiceOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IConfiguration>();
            var path = $"{nameof(SinksTelemetryAzureTablesServiceOptions)}";
            configuration.Bind(path, options);
        }
    }
}
