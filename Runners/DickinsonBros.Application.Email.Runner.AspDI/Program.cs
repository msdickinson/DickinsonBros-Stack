using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Adapter.AspDI.Extensions;
using DickinsonBros.Application.Email.Runner.AspDI.Config;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.File.AspDI.Extensions;
using DickinsonBros.Infrastructure.SMTP.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace DickinsonBros.Application.Email.Runner.AspDI
{
    public class ItemWithCallBackMethod
    {
        public string Input;
        public Task CallBackMethod;
    }

    class Program
    {
        private readonly ConcurrentQueue<ItemWithCallBackMethod> concurrentQueue = new ConcurrentQueue<ItemWithCallBackMethod>();

        async static Task Main()
        {
            await new Program().DoMain();
        }
        public async Task ExecuteAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            finally
            {
            }
        }

        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                using var provider = serviceCollection.BuildServiceProvider();
                var emailService = provider.GetRequiredService<IEmailService<RunnerSMTPServiceOptionsType>>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();

                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Create Item

                //IsValidEmailFormat
                Console.WriteLine($"IsValidEmailFormat(\"SampleEmail@email.com\"): {emailService.IsValidEmailFormat("SampleEmail@email.com")}");
                Console.WriteLine($"IsValidEmailFormat(\"SampleEmail\"): {emailService.IsValidEmailFormat("SampleEmail")}");

                //SendEmail
                Console.WriteLine($"SendEmailAsync");
                await emailService.SendAsync(CreateMessage()).ConfigureAwait(false);

                //Close
                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private MimeMessage CreateMessage()
        {
            var email = "marksamdickinson@gmail.com";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test", email));
            message.To.Add(new MailboxAddress("Test", email));
            message.Subject = "Test Runner - Email Service";

            message.Body = new TextPart("plain")
            {
                Text = $@"Test Runner Email Body"
            };
            var emailUri = new Uri($"mailto:{email}");
            var emailHost = emailUri.Host;

            return message;
        }
   
        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            //--Misc
            serviceCollection.TryAddSingleton<IFileSystem, FileSystem>();

            //--Core
            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();

            //--Encryption
            serviceCollection.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            serviceCollection.AddSMTPService<RunnerSMTPServiceOptionsType, Configuration>();
            serviceCollection.AddFileService();
            serviceCollection.AddEmailService<RunnerSMTPServiceOptionsType>();
            serviceCollection.AddSMTPService<RunnerSMTPServiceOptionsType, Configuration>();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();


            return serviceCollection;
        }
    }
}
