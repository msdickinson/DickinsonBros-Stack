using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Logger
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Logger", Group = "Core")]
    public class LoggerIntegrationTests : ILoggerIntegrationTests
    {
        public ILoggerService<LoggerIntegrationTests> _loggerService;

        public LoggerIntegrationTests
        (
            ILoggerService<LoggerIntegrationTests> loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async Task LogDebugRedacted_NoData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";

            _loggerService.LogDebugRedacted(message, LogGroup.Application);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogDebugRedacted_WithData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";
            var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };

            _loggerService.LogDebugRedacted(message, LogGroup.Application, data);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogInformationRedacted_NoData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";

            _loggerService.LogInformationRedacted(message, LogGroup.Application);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogInformationRedacted_WithData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";
            var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };

            _loggerService.LogInformationRedacted(message, LogGroup.Application, data);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogWarningRedacted_NoData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";

            _loggerService.LogWarningRedacted(message, LogGroup.Application);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogWarningRedacted_WithData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";
            var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };

            _loggerService.LogWarningRedacted(message, LogGroup.Application, data);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogErrorRedacted_NoData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";
            var exception = new Exception("Error");

            _loggerService.LogErrorRedacted(message, LogGroup.Application, exception);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task LogErrorRedacted_WithData_DoesNotThrowNull(List<string> successLog)
        {
            var message = "Generic Log Message";
            var data = new Dictionary<string, object>
                                {
                                    { "Username", "DemoUser" },
                                    { "Password",
@"{
""Password"": ""password""
}"
                                    }
                                };
            var exception = new Exception("Error");

            _loggerService.LogErrorRedacted(message, LogGroup.Application, exception, data);
            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
