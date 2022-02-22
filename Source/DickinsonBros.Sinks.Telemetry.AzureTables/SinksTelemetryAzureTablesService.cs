using Dickinsonbros.Core.Guid.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables
{
    public class SinksTelemetryAzureTablesService<T> : ISinksTelemetryAzureTablesService<T>
    where T : AzureTableServiceOptionsType
    {
        //Services
        internal readonly IAzureTableService<T> _azureTableService;    
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly ILoggerService<SinksTelemetryAzureTablesService<T>> _loggerService;
        internal readonly IGuidService _guidService;
        internal readonly IHostApplicationLifetime _hostApplicationLifetime;

        //Other
        internal readonly ConcurrentQueue<TelemetryDataEntity> _queueTelemetryDataEntitys = new ConcurrentQueue<TelemetryDataEntity>();
        internal readonly CancellationTokenSource _internalTokenSource = new CancellationTokenSource();
        internal readonly SinksTelemetryAzureTablesServiceOptions _options = new SinksTelemetryAzureTablesServiceOptions();
        internal Task _uploaderTask;

        public SinksTelemetryAzureTablesService
        (
            IOptions<SinksTelemetryAzureTablesServiceOptions> options,
            IGuidService guidService,
            ILoggerService<SinksTelemetryAzureTablesService<T>> loggerService,
            ITelemetryWriterService telemetryWriterService,
            IAzureTableService<T> azureTableService,
            IHostApplicationLifetime hostApplicationLifetime
        )
        {
            _loggerService = loggerService;
            _guidService = guidService;
            _options = options.Value;
            _azureTableService = azureTableService;
            _telemetryWriterService = telemetryWriterService;
            _hostApplicationLifetime = hostApplicationLifetime;

            _telemetryWriterService.NewTelemetryEvent += NewTelemetryEvent;
            _hostApplicationLifetime.ApplicationStopped.Register(() => FlushAsync().Wait());
            _uploaderTask = Uploader(CancellationTokenSource.CreateLinkedTokenSource(hostApplicationLifetime.ApplicationStopping, _internalTokenSource.Token).Token);
            _uploaderTask.ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            //Cancel Token
            _internalTokenSource.Cancel();

            //Complete Running Process
            await _uploaderTask.ConfigureAwait(false);

            //Run Once more to flush out any left.
            await UploadAsync().ConfigureAwait(false);
        }

        internal void NewTelemetryEvent(TelemetryItem telemetryItem)
        {
            _queueTelemetryDataEntitys.Enqueue(new TelemetryDataEntity
            {
                PartitionKey = _options.PartitionKey,
                UserStory = telemetryItem.UserStory,
                RowKey = _guidService.NewGuid().ToString(),
                EventTimestamp = telemetryItem.DateTimeUTC,
                ElapsedMilliseconds = (int)telemetryItem.Duration.TotalMilliseconds,
                Source = telemetryItem.Source,
                Connection = telemetryItem.Connection,
                Request = telemetryItem.Request,
                TelemetryState = telemetryItem.TelemetryResponseState.ToString(),
                TelemetryType = telemetryItem.TelemetryType.ToString(),
                CorrelationId = telemetryItem.CorrelationId
            });
        }

        internal async Task UploadAsync()
        {
            var telemetryItems = new List<TelemetryDataEntity>();
            while (_queueTelemetryDataEntitys.TryDequeue(out TelemetryDataEntity telemetryItem))
            {
                telemetryItems.Add(telemetryItem);
            }

            if (!telemetryItems.Any())
            {
                return;
            }

            await _azureTableService.InsertBulkAsync(telemetryItems, _options.TableName, false).ConfigureAwait(false);
        }

        internal async Task Uploader(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_options.UploadInterval, token).ConfigureAwait(false);
                    await UploadAsync().ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    _loggerService.LogInformationRedacted("Uploader Task Canceled", LogGroup.Infrastructure);
                }
                catch (Exception ex)
                {
                    _loggerService.LogErrorRedacted($"Unhandled exception in {nameof(Uploader)}",LogGroup.Infrastructure, ex);
                }
            }
        }

    }
}
