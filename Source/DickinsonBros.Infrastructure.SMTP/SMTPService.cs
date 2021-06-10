using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.SMTP.Abstractions;
using DickinsonBros.Infrastructure.SMTP.Abstractions.Models;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SMTP
{
    public class SMTPService<T> : ISMTPService<T>
    where T : SMTPServiceOptionsType
    {
        internal bool ExitOnFlush = false;
        internal readonly ISMTPClientFactory _smtpClientFactory;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ICorrelationService _correlationService;
        internal readonly ILoggerService<SMTPService<T>> _logger;
        internal readonly IHostApplicationLifetime _hostApplicationLifetime;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly SMTPServiceOptions _smtpServiceOptions;

        internal readonly ConcurrentQueue<EmailRequest> emailRequests = new ConcurrentQueue<EmailRequest>();

        internal readonly ConcurrentDictionary<Task, string> smtpClientHandlers = new ConcurrentDictionary<Task, string>();
        internal readonly object _object = new object();

        public SMTPService
        (
            ISMTPClientFactory smtpClientFactory,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory,
            ICorrelationService correlationService,
            ILoggerService<SMTPService<T>> logger,
            ITelemetryWriterService telemetryWriterService,
            IOptions<SMTPServiceOptions<T>> smtpServiceOptions,
            IHostApplicationLifetime hostApplicationLifetime
        )
        {
            _smtpClientFactory = smtpClientFactory;
            _dateTimeService = dateTimeService;
            _stopwatchFactory = stopwatchFactory;
            _correlationService = correlationService;
            _logger = logger;
            _telemetryWriterService = telemetryWriterService;
            _smtpServiceOptions = smtpServiceOptions.Value;

            _hostApplicationLifetime = hostApplicationLifetime;
            _hostApplicationLifetime.ApplicationStopped.Register(() => FlushAsync().Wait());
        }
        public async Task<SendEmailDescriptor> SendEmailAsync(MimeMessage mimeMessage)
        {
            var methodIdentifier = $"{nameof(SMTPService<T>)}<{typeof(T).Name}>.{nameof(SMTPService<T>.SendEmailAsync)}";

            if (mimeMessage == null)
            {
                throw new ArgumentNullException(nameof(mimeMessage));
            }

            var stopWatch = _stopwatchFactory.NewStopwatchService();
            stopWatch.Start();
            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _smtpServiceOptions.Host,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Send {mimeMessage.Subject}",
                TelemetryType = TelemetryType.SMTP,
                CorrelationId = _correlationService.CorrelationId
            };

            var emailRequest = new EmailRequest
            {
                MimeMessage = mimeMessage,
                CallBack = new Task(async () => await Task.CompletedTask.ConfigureAwait(false))
            };

            try
            {
                //Enqueue
                emailRequests.Enqueue(emailRequest);
               
                //Invoke Queue
                BalanceSmtpClients();

                //Ensure it doesnt invoke it.
                var result = await Task.WhenAny(emailRequest.CallBack).ConfigureAwait(false);
                stopWatch.Stop();

                insertTelemetryRequest.Duration = stopWatch.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = emailRequest.SendEmailDescriptor.SendEmailResult == SendEmailResult.Successful ? TelemetryResponseState.Successful : TelemetryResponseState.UnhandledException;

                if (emailRequest.SendEmailDescriptor.Exception != null)
                {                
                    _logger.LogErrorRedacted
                    (
                        methodIdentifier,
                        LogGroup.Infrastructure,
                        emailRequest.SendEmailDescriptor.Exception,
                        new Dictionary<string, object>
                        {
                            { nameof(_smtpServiceOptions.Host), _smtpServiceOptions.Host },
                            { nameof(emailRequest.MimeMessage.Subject), emailRequest.MimeMessage.Subject },
                            { nameof(emailRequest.MimeMessage.To), string.Join(", ", emailRequest.MimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address))},
                            { nameof(emailRequest.MimeMessage.From), string.Join(", ", emailRequest.MimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address))},
                            { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                            { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                        }
                    );
                }
                else
                {   
                    _logger.LogInformationRedacted
                    (
                        methodIdentifier,
                        LogGroup.Infrastructure,
                        new Dictionary<string, object>
                        {
                            { nameof(_smtpServiceOptions.Host), _smtpServiceOptions.Host },
                            { nameof(emailRequest.MimeMessage.Subject), emailRequest.MimeMessage.Subject },
                            { nameof(emailRequest.MimeMessage.To), string.Join(", ", emailRequest.MimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address))},
                            { nameof(emailRequest.MimeMessage.From), string.Join(", ", emailRequest.MimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address))},
                            { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                            { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                        }
                    );
                }
                return emailRequest.SendEmailDescriptor;
            }

            catch(Exception exception)
            {
                stopWatch.Stop();
                insertTelemetryRequest.Duration = stopWatch.Elapsed;
                insertTelemetryRequest.TelemetryResponseState =  TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                   methodIdentifier,
                   LogGroup.Infrastructure,
                   exception,
                   new Dictionary<string, object>
                   {
                        { nameof(_smtpServiceOptions.Host), _smtpServiceOptions.Host },
                        { nameof(emailRequest.MimeMessage.Subject), emailRequest.MimeMessage.Subject },
                        { nameof(emailRequest.MimeMessage.To), string.Join(", ", emailRequest.MimeMessage.To.OfType<MailboxAddress>().Select(e => e.Address))},
                        { nameof(emailRequest.MimeMessage.From), string.Join(", ", emailRequest.MimeMessage.From.OfType<MailboxAddress>().Select(e => e.Address))},
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                   }
               );
                return new SendEmailDescriptor
                {
                    Exception = exception,
                    SendEmailResult = SendEmailResult.Failed
                };
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
          
        }

        internal void BalanceSmtpClients()
        {
            //Ensure this is called by one thread at a time. (without this you could over add connections)
            lock (_object)
            {
                //Cut out if already maxed out
                if (smtpClientHandlers.Count >= _smtpServiceOptions.MaxConnections)
                {
                    return;
                }

                //Determine ideal clients
                var idealClients = Math.Min
                (
                    (emailRequests.Count / _smtpServiceOptions.IdealEmailLoad) + (emailRequests.Any() ? 1 : 0),
                    _smtpServiceOptions.MaxConnections
                );

                //Determine connections to add
                var connectionsToAdd = idealClients - smtpClientHandlers.Count;
                for (int i = 0; i < connectionsToAdd; i++)
                {
                    var task = SmtpClientProcessQueueItemsAsync();
                    smtpClientHandlers.TryAdd(task, "");
                    HandledAwaited(task);
                }
            }
        }
        
        internal async void HandledAwaited(Task task)
        {
            try
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                        _logger.LogErrorRedacted
                        (
                            $"{nameof(SMTPService<T>)}<{typeof(T).Name}>.{nameof(SMTPService<T>.HandledAwaited)}",
                            LogGroup.Infrastructure,
                            exception
                        );
                }
                finally
                {
                    smtpClientHandlers.TryRemove(task, out _);

                    //If we are removing in item from the pool we do not want to add more during a flush
                    if (!ExitOnFlush)
                    {
                        BalanceSmtpClients();
                    }
                }
            }
            catch (Exception)
            {
                //In case The catch log error or the finnaly throws
                //I do not log here in case the logger throws. I had to stop somewhere.
            }
        }

        internal async Task SmtpClientProcessQueueItemsAsync()
        {
            var methodIdentifier = $"{nameof(SMTPService<T>)}<{typeof(T).Name}>.{nameof(SMTPService<T>.SmtpClientProcessQueueItemsAsync)}";
            var emailRequest = (EmailRequest)null;
            var stopWatch = (IStopwatchService)null;

            try
            {            
                stopWatch = _stopwatchFactory.NewStopwatchService();

                //Create Client
                using (var smtpClient = _smtpClientFactory.Create())
                {
                    //Connect
                    smtpClient.Timeout = (int)_smtpServiceOptions.EmailTimeout.TotalMilliseconds;

                    await smtpClient.ConnectAsync(_smtpServiceOptions.Host, _smtpServiceOptions.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
                    await smtpClient.AuthenticateAsync(_smtpServiceOptions.UserName, _smtpServiceOptions.Password).ConfigureAwait(false);

                    //Run untill InactivityTimeout is reached
                    stopWatch.Start();
                    while (stopWatch.Elapsed < _smtpServiceOptions.InactivityTimeout)
                    {
                        while (emailRequests.TryDequeue(out emailRequest))
                        {
                            //Send Email
                            await smtpClient.SendAsync(emailRequest.MimeMessage).ConfigureAwait(false);

                            //Signal Call back
                            emailRequest.SendEmailDescriptor = new SendEmailDescriptor
                            {
                                SendEmailResult = SendEmailResult.Successful
                            };
                            emailRequest.CallBack.Start();

                            //clear emailRequest
                            emailRequest = null;

                            //Restart Inactivity stopwatch
                            stopWatch.Reset();
                            stopWatch.Start();
                        }

                        if (ExitOnFlush)
                        {
                            await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
                            return;
                        }

                        await Task.Delay(_smtpServiceOptions.PullingDelay);
                    }
                    await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
                }
                  
            }
            catch (Exception exception)
            {
                stopWatch.Stop();

                if (emailRequest is null)
                {
                    _logger.LogErrorRedacted
                    (
                        methodIdentifier,
                        LogGroup.Infrastructure,
                        exception
                    );
                    return;
                }

                //Signal call back
                emailRequest.SendEmailDescriptor = new SendEmailDescriptor
                {
                    Exception = exception,
                    SendEmailResult = SendEmailResult.Failed
                };

                if(emailRequest.CallBack.Status == TaskStatus.Created)
                {
                    emailRequest.CallBack.Start();
                }
            }
        }

        internal async Task FlushAsync()
        {
            //Trigger tasks to exit when there are no emails left
            ExitOnFlush = true;

            //Wait for all connections to close
            await Task.WhenAll(smtpClientHandlers.Select(e=> e.Key));
        }
    }

    public class EmailRequest
    {
        public MimeMessage MimeMessage { get; set; }
        public Task CallBack { get; set; }
        public SendEmailDescriptor SendEmailDescriptor { get; set; }
    }
  
}
