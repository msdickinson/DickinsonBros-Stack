using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables
{
    public class SinksTelemetryAzureTablesService<U> : ITelemetryServiceWriter
    where U : AzureTableServiceOptionsType
    {
        //internal readonly IAzureTableService<U> _azureTableService;
        //internal readonly ConcurrentQueue<InsertTelemetryRequest> _queueInsertTelemetryRequest = new ConcurrentQueue<InsertTelemetryRequest>();
        //internal CancellationTokenSource _internalTokenSource = new CancellationTokenSource();

        //public SinksTelemetryAzureTablesService
        //(
        //    IAzureTableService<U> azureTableService,
        //    IHostApplicationLifetime hostApplicationLifetime
        //)
        //{
        //    _azureTableService = azureTableService;

        //    _telemetryService.NewTelemetryEvent += TelemetryService_NewTelemetryEvent;
        //    _internalTokenSource = new CancellationTokenSource();
        //    hostApplicationLifetime.ApplicationStopped.Register(() => FlushAsync().Wait());
        //}

        //public void Insert(InsertTelemetryRequest telemetryItem)
        //{
        //    _queueInsertTelemetryRequest.Enqueue(telemetryItem);
        //}

        //internal async Task Uploader(CancellationToken token)
        //{
        //    while (!token.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            await Task.Delay(_options.UploadInterval, token).ConfigureAwait(false);
        //            await UploadAsync().ConfigureAwait(false);
        //        }
        //        catch (TaskCanceledException)
        //        {
        //            _loggingService.LogInformationRedacted("Uploader Task Canceled");
        //        }
        //        catch (Exception ex)
        //        {
        //            _loggingService.LogErrorRedacted($"Unhandled exception in {nameof(Uploader)}", ex);
        //        }
        //    }
        //}

        //public async Task<IEnumerable<TableResult<TelemetryDataEntity>>> UploadAsync()
        //{
        //    var telemetryItems = new List<TelemetryDataEntity>();
        //    while (_queueTelemetry.TryDequeue(out TelemetryData telemetryData))
        //    {
        //        telemetryItems.Add(new TelemetryDataEntity
        //        {
        //            PartitionKey = PARTITION_KEY,
        //            RowKey = _guidService.NewGuid().ToString(),
        //            DateTime = telemetryData.DateTime,
        //            ElapsedMilliseconds = telemetryData.ElapsedMilliseconds,
        //            Name = telemetryData.Name,
        //            Source = telemetryData.Source,
        //            TelemetryState = telemetryData.TelemetryState.ToString(),
        //            TelemetryType = telemetryData.TelemetryType.ToString(),
        //        });
        //    }

        //    if (!telemetryItems.Any())
        //    {
        //        return new List<TableResult<TelemetryDataEntity>>();
        //    }

        //    return await _azureTableService.InsertBulkAsync(telemetryItems, _options.TableName).ConfigureAwait(false);
        //}

        //public async Task FlushAsync()
        //{
        //    //Cancel Token
        //    _internalTokenSource.Cancel();

        //    //Complete Running Process
        //   // await _uploaderTask.ConfigureAwait(false);

        //    //Run Once more to flush out any left.
        //    await UploadAsync().ConfigureAwait(false);
        //}
        public Task FlushAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Insert(InsertTelemetryRequest telemetryItem)
        {
            throw new System.NotImplementedException();
        }
    }
}
