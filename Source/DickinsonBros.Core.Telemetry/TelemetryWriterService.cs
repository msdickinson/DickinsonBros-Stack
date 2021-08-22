using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace DickinsonBros.Core.Telemetry
{
    public class TelemetryWriterService : ITelemetryWriterService
    {
        internal readonly TelemetryWriterServiceOptions _telemetryServiceWriterOptions;

        public string ScopedUserStory
        {
            get
            {
                return _asyncLocalUserStory.Value;
            }
            set
            {
                _asyncLocalUserStory.Value = value;
            }
        }
        internal AsyncLocal<string> _asyncLocalUserStory { get; set; }


        public TelemetryWriterService
        (
            IOptions<TelemetryWriterServiceOptions> telemetryServiceWriterOptions
        ) 
        {
            _telemetryServiceWriterOptions = telemetryServiceWriterOptions.Value;
            _asyncLocalUserStory = new AsyncLocal<string>();
        }

        public delegate void NewTelemetryEventHandler(TelemetryItem telemetryItem);
        event ITelemetryWriterService.NewTelemetryItemEventHandler NewTelemetryEvent;

        [ExcludeFromCodeCoverage]
        event ITelemetryWriterService.NewTelemetryItemEventHandler ITelemetryWriterService.NewTelemetryEvent
        {
            add => NewTelemetryEvent += value;
            remove => NewTelemetryEvent -= value;
        }

        public void Insert(InsertTelemetryItem insertTelemetryItem)
        {
            if (insertTelemetryItem == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(insertTelemetryItem.ConnectionName))
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(insertTelemetryItem.Request))
            {
                throw new ArgumentNullException();
            }

            if (insertTelemetryItem.DateTimeUTC == DateTime.MinValue)
            {
                throw new ArgumentException("Date Expected to be set", nameof(insertTelemetryItem.DateTimeUTC));
            }

            NewTelemetryEvent?.Invoke(new TelemetryItem
            {
                Source = _telemetryServiceWriterOptions.ApplicationName,
                UserStory = ScopedUserStory,
                Connection = insertTelemetryItem.ConnectionName,
                DateTimeUTC = insertTelemetryItem.DateTimeUTC,
                Duration = insertTelemetryItem.Duration,
                Request = insertTelemetryItem.Request,
                TelemetryResponseState = insertTelemetryItem.TelemetryResponseState,
                TelemetryType = insertTelemetryItem.TelemetryType,
                CorrelationId = insertTelemetryItem.CorrelationId
            });
        }
     
    }
}