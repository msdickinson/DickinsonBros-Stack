using DickinsonBros.Application.Email;
using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Test.Integration.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Application.Email
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Email", Group = "Application")]
    public class EmailIntegrationTests : IEmailIntegrationTests
    {
        public readonly ITelemetryWriterService _telemetryWriterService;
        public readonly IEmailService<RunnerSMTPServiceOptionsType> _emailService;
        public readonly IOptions<EmailServiceOptions<RunnerSMTPServiceOptionsType>> _emailServiceOptions;
        public readonly ICorrelationService _correlationService;


        public EmailIntegrationTests
        (
            IEmailService<RunnerSMTPServiceOptionsType> emailService,
            IOptions<EmailServiceOptions<RunnerSMTPServiceOptionsType>> emailServiceOptions,
            ICorrelationService correlationService,
            ITelemetryWriterService telemetryWriterService
        )
        {
            _emailService = emailService;
            _emailServiceOptions = emailServiceOptions;
            _correlationService = correlationService;
            _telemetryWriterService = telemetryWriterService;
        }

        public async Task SendEmailAsync_Runs_FileSavedAndSMTPSuccessful(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "Telemetry";

            //Create Item
            var mimeMessage = CreateMessage(0);

            //Add To Queue
            var result = await _emailService.SendAsync(mimeMessage).ConfigureAwait(false);

            Assert.IsTrue(result.SavedEmailToDisk, "Email did not save to disk");
            successLog.Add($"Save to file was successful");

            Assert.IsTrue(result.AttemptedSMTP && result.SendEmailDescriptor.SendEmailResult == SendEmailResult.Successful, $"SMTP Failure: {result.SendEmailDescriptor.Exception?.Message}");
            successLog.Add($"SMTP was successful successful");
        }

        private MimeMessage CreateMessage(int counter)
        {
            var email = "marksamdickinson@gmail.com";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test", email));
            message.To.Add(new MailboxAddress("Test", email));
            message.Subject = $"EmailIntegrationTests";
            message.Body = new TextPart("plain")
            {
                Text = $@"Test Runner Email Body {_correlationService.CorrelationId} - ({counter})"
            };
          
            return message;
        }

    }
}
