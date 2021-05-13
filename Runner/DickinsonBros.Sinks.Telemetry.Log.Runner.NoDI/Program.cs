using DickinsonBros.Core.Correlation;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Logger;
using DickinsonBros.Core.Redactor;
using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Redactor.Models;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.Log.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var sinksTelemetryLogService = CreateSinksTelemetryLogService();

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = "SampleConnectionName",
                DateTimeUTC = DateTime.UtcNow,
                Duration = TimeSpan.FromSeconds(100),
                SignalRequest = "SampleSignalRequest",
                SignalResponse = "SampleSignalResponse",
                TelemetryResponseState = TelemetryResponseState.Successful,
                TelemetryType = TelemetryType.Application
            };

            await sinksTelemetryLogService.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
        private SinksTelemetryLogService CreateSinksTelemetryLogService()
        {
            var loggerService = CreateLoggerService();
            var sinksTelemetryLogService = new SinksTelemetryLogService(loggerService);
          
            return sinksTelemetryLogService;
        }
        private LoggerService<SinksTelemetryLogService> CreateLoggerService()
        {
            var correlationService = CreateCorrelationService();
            var redactorService = CreateRedactorService();

            var baseLogger = new Logger<SinksTelemetryLogService>(LoggerFactory.Create(builder => builder.AddConsole()));
            return new LoggerService<SinksTelemetryLogService>(baseLogger, redactorService, correlationService);
        }

        private IRedactorService CreateRedactorService()
        {
            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[]
                {
                    "Password"
                },
                RegexValuesToRedact = new string[]
                {
                }
            };
            var options = Options.Create(redactorServiceOptions);

            return new RedactorService(options);
        }

        private ICorrelationService CreateCorrelationService()
        {
            return new CorrelationService();
        }
    }
}
