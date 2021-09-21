using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Application.Email.Abstractions;
using DickinsonBros.Application.Email.Abstractions.Models;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Infrastructure.File.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DickinsonBros.Application.Email
{
    public class EmailService<T> : IEmailService<T>
    where T : SMTPServiceOptionsType
    {
        internal readonly EmailServiceOptions<T> _emailServiceOptions;
        internal readonly IGuidService _guidService;
        internal readonly ISMTPService<T> _smtpService;
        internal readonly IFileService _fileService;
        internal readonly ILoggerService<EmailService<T>> _loggerService;

        public EmailService
        (
            IOptions<EmailServiceOptions<T>> emailServiceOptions,
            IGuidService guidService,
            ISMTPService<T> smtpService,
            IFileService fileService,
            ILoggerService<EmailService<T>> loggerService
        )
        {
            _emailServiceOptions = emailServiceOptions.Value;
            _guidService = guidService;
            _smtpService = smtpService;
            _fileService = fileService;
            _loggerService = loggerService;
        }

        [ExcludeFromCodeCoverage]
        public bool IsValidEmailFormat(string email)
        {
            var methodIdentifier = $"{nameof(EmailService<T>)}<{typeof(T).Name}>.{nameof(EmailService<T>.IsValidEmailFormat)}";

            var isVaild = false;
            var regexMatchTimeout = false;

            if (string.IsNullOrWhiteSpace(email))
            {
                isVaild = false;
            }
            else
            {
                try
                {
                    isVaild = Regex.IsMatch(email,
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    regexMatchTimeout = true;
                    isVaild = false;
                }
            }

            _loggerService.LogInformationRedacted
            (
                methodIdentifier,
                LogGroup.Infrastructure,
                new Dictionary<string, object>
                {
                    { nameof(email), email },
                    { nameof(isVaild), isVaild },
                    { nameof(regexMatchTimeout), regexMatchTimeout }
                }
            );

            return isVaild;
        }

        public async Task<SendAsyncDescriptor> SendAsync(MimeMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var sendAsyncDescriptor = new SendAsyncDescriptor();
            var smtpTask = (Task<SendEmailDescriptor>)null;

            var tasks = new List<Task>();

            if (_emailServiceOptions.SaveToFile)
            {
                using (var memory = new MemoryStream())
                {
                    message.WriteTo(memory);
                    var byteArray = memory.ToArray();
                    var path = _emailServiceOptions.SaveDirectory + "\\" + _guidService.NewGuid().ToString() + ".eml";
                    var saveFileTask = _fileService.UpsertFileAsync(path, byteArray);

                    sendAsyncDescriptor.SavedEmailToDisk = true;
                    tasks.Add(saveFileTask);
                }
            }

            if (_emailServiceOptions.SendSMTP)
            {
                smtpTask = _smtpService.SendEmailAsync(message);
                sendAsyncDescriptor.AttemptedSMTP = true;
                tasks.Add(smtpTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            sendAsyncDescriptor.SendEmailDescriptor = smtpTask?.Result;

            return sendAsyncDescriptor;
        }
    }
}
