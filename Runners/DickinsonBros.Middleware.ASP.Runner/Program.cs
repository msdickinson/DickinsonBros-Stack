using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Middleware.ASP.Runner.Config;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace DickinsonBros.Middleware.ASP.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerService<Program> logger = null;
            try
            {
                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();

                var loggerService = (ILoggerService<Program>)host.Services.GetService(typeof(ILoggerService<Program>));
                var sinksTelemetryLogService = (ISinksTelemetryLogService)host.Services.GetService(typeof(ISinksTelemetryLogService));
                var sinksTelemetryAzureTablesService = (ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>)host.Services.GetService(typeof(ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>));

                host.Run();
            }
            catch (Exception exception)
            {
                logger?.LogErrorRedacted($"Error In Main of {typeof(Program).Namespace}.{nameof(Program)}", Core.Logger.Abstractions.Models.LogGroup.Infrastructure, exception);
            }

            logger?.LogInformationRedacted("Middleware ASP Runner - Closing", Core.Logger.Abstractions.Models.LogGroup.Infrastructure);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
