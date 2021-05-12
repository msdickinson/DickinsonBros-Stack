using DickinsonBros.Core.Correlation;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Redactor;
using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Redactor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Logger.Logger.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var loggingService = CreateLoggerService();

            var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };

            var message = "Generic Log Message";
            var exception = new Exception("Error");

            loggingService.LogDebugRedacted(message, Abstractions.Models.LogGroup.Application);
            loggingService.LogDebugRedacted(message, Abstractions.Models.LogGroup.Application, data);

            loggingService.LogInformationRedacted(message, Abstractions.Models.LogGroup.Application);
            loggingService.LogInformationRedacted(message, Abstractions.Models.LogGroup.Application, data);

            loggingService.LogWarningRedacted(message, Abstractions.Models.LogGroup.Application);
            loggingService.LogWarningRedacted(message, Abstractions.Models.LogGroup.Application, data);

            loggingService.LogErrorRedacted(message, Abstractions.Models.LogGroup.Application, exception);
            loggingService.LogErrorRedacted(message, Abstractions.Models.LogGroup.Application, exception, data);

            await Task.CompletedTask.ConfigureAwait(false);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private LoggerService<Program> CreateLoggerService()
        {
            var correlationService = CreateCorrelationService();
            var redactorService = CreateRedactorService();

            var baseLogger = new Logger<Program>(LoggerFactory.Create(builder => builder.AddConsole()));
            return new LoggerService<Program>(baseLogger, redactorService, correlationService);
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
