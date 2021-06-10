using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.DNS.Abstractions;
using DickinsonBros.Infrastructure.DNS.Abstractions.Models;
using DnsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.DNS
{
    public class DNSService : IDNSService
    {
        internal readonly ICorrelationService _correlationService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ILoggerService<IDNSService> _logger;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly ILookupClient _lookupClient;

        public DNSService
        (
            ICorrelationService correlationService,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory,
            ITelemetryWriterService telemetryWriterService,
            ILoggerService<IDNSService> logger,
            ILookupClient lookupClient
        )
        {
            _correlationService = correlationService;
            _dateTimeService = dateTimeService;
            _telemetryWriterService = telemetryWriterService;
            _stopwatchFactory = stopwatchFactory;
            _logger = logger;
            _lookupClient = lookupClient;
        }


        public async Task<ValidateEmailDomainResult> ValidateEmailDomainAsync(string emailDomain)
        {
            var methodIdentifier = $"{nameof(DNSService)}.{nameof(DNSService.ValidateEmailDomainAsync)}";
            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "ILookupClient Domain Servers",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = nameof(DNSService.ValidateEmailDomainAsync),
                TelemetryType = TelemetryType.DomainNameServer,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var result = (await _lookupClient.QueryAsync(emailDomain, QueryType.MX).ConfigureAwait(false))
                                .Answers.MxRecords().Any()
                                ? ValidateEmailDomainResult.Vaild : ValidateEmailDomainResult.Invaild;

                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = result == ValidateEmailDomainResult.Vaild ? TelemetryResponseState.Successful : TelemetryResponseState.NotFound;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(emailDomain), emailDomain },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return result;
            }
            catch(Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(emailDomain), emailDomain },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return ValidateEmailDomainResult.UnableToReachDNS;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }
    }
}
