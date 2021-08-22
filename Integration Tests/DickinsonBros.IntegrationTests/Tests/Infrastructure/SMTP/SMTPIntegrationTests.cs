using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.SMTP
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "SMTP", Group = "Infrastructure")]
    public class SMTPIntegrationTests : ISMTPIntegrationTests
    {
        public readonly ITelemetryWriterService _telemetryWriterService;
        public readonly ISMTPService<RunnerSMTPServiceOptionsType> _smtpService;
        public readonly ICorrelationService _correlationService;


        public SMTPIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService,
            ISMTPService<RunnerSMTPServiceOptionsType> smtpService,
            ICorrelationService correlationService
        )
        {
            _telemetryWriterService = telemetryWriterService;
            _smtpService = smtpService;
            _correlationService = correlationService;
        }

        public async Task SendEmailAsync_Runs_ExpectedReturnsAndNoThrows(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "SMTP";

            //Create Item
            var mimeMessage = CreateMessage(0);

            //Add To Queue
            var sendEmailDescriptor = await _smtpService.SendEmailAsync(mimeMessage);
            
            Assert.AreEqual(SendEmailResult.Successful, sendEmailDescriptor.SendEmailResult, $"SendEmailResult is not Successful. SendEmailResult is {sendEmailDescriptor.SendEmailResult}");
            successLog.Add($"SendEmailResult is Successful."); 
            
            Assert.IsNull(sendEmailDescriptor.Exception, "Exception is not null");
            successLog.Add($"Exception is null.");
        }

        public async Task SendEmailAsync_BulkSend_ExpectedReturnsAndNoThrows(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "SMTP";

            int counter = 1;

            //Add To Queue
            var results = await Task.WhenAll
            (
                _smtpService.SendEmailAsync(CreateMessage(counter++)),
                _smtpService.SendEmailAsync(CreateMessage(counter++))
            );
            var x = results.All(e => e.SendEmailResult == SendEmailResult.Successful);
            Assert.IsTrue(results.All(e=> e.SendEmailResult == SendEmailResult.Successful), $"SendEmailResults are not all Successful. SendEmailResults are '{String.Join(",", results.Select(e=> e.SendEmailResult.ToString()))}'");
            successLog.Add($"SendEmailResults are Successful.");

            Assert.IsTrue(results.All(e => e.Exception == null), $"Exceptions found '{results.Where(e=> e.Exception != null).Select(e=> e.Exception.ToString())}'");
            successLog.Add($"Exceptions are null.");
        }

        private MimeMessage CreateMessage(int counter)
        {
            var email = "marksamdickinson@gmail.com";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test", email));
            message.To.Add(new MailboxAddress("Test", email));
            message.Subject = $"SMTPIntegrationTests";
            message.Body = new TextPart("plain")
            {
                Text = $@"Test Runner Email Body {_correlationService.CorrelationId} - ({counter})"
            };
          
            return message;
        }

    }
}
