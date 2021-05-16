using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Factories;
using DickinsonBros.Infrastructure.AzureTables.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DickinsonBros.Core.Logger.Abstractions;
namespace DickinsonBros.Infrastructure.AzureTables
{
    public class AzureTableService<U> : IAzureTableService<U>
    where U : AzureTableServiceOptionsType
    {
        internal ILoggerService<U> _loggerService;
        internal CloudStorageAccount _cloudStorageAccount;
        internal CloudTableClient _cloudTableClient;
        internal IDateTimeService _dateTimeService;
        internal IStopwatchFactory _stopwatchFactory;
        internal ITelemetryServiceWriter _telemetryServiceWriter;

        public AzureTableService
        (
            ILoggerService<U> loggerService,
            ICloudStorageAccountFactory cloudStorageAccountFactory,
            ICloudTableClientFactory cloudTableClientFactory,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory, 
            ITelemetryServiceWriter telemetryServiceWriter,
            IOptions<AzureTableServiceOptions<U>> options
        )
        {
            _loggerService = loggerService;
            _cloudStorageAccount = cloudStorageAccountFactory.CreateCloudStorageAccount(options.Value.ConnectionString);
            _cloudTableClient = cloudTableClientFactory.CreateCloudTableClient(_cloudStorageAccount);
            _dateTimeService = dateTimeService;
            _stopwatchFactory = stopwatchFactory;
            _telemetryServiceWriter = telemetryServiceWriter;
        }

        public async Task<TableResult<T>> DeleteAsync<T>(T item, string tableName) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.DeleteAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Delete {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var deleteOperation = TableOperation.Delete(item);
                var tableResult = await cloudTable.ExecuteAsync(deleteOperation).ConfigureAwait(false);
                stopwatchService.Stop();

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results);

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }
        public async Task<IEnumerable<TableBatchResult>> DeleteBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.DeleteAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"DeleteBulk {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var continuationToken = (TableContinuationToken)null;
                var results = new List<TableBatchResult>();
                do
                {
                    var cloudTable = _cloudTableClient.GetTableReference(tableName);

                    // Split into chunks of 100 for batching
                    List<List<T>> rowsChunked = items.Select((x, index) => new { Index = index, Value = x })
                        .Where(x => x.Value != null)
                        .GroupBy(x => x.Index / 100)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();

                    // Delete each chunk of 100 in a batch
                    foreach (List<T> rows in rowsChunked)
                    {
                        TableBatchOperation tableBatchOperation = new TableBatchOperation();
                        rows.ForEach(x => tableBatchOperation.Add(TableOperation.Delete(x)));

                        results.Add(await cloudTable.ExecuteBatchAsync(tableBatchOperation));
                    }
                }
                while (continuationToken != null);

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }

        }
    
        public async Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.FetchAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Fetch {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowkey);
                var tableResult = await cloudTable.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

                stopwatchService.Stop();

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results);

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }
       
        public async Task<TableResult<T>> InsertAsync<T>(T item, string tableName) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.InsertAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Insert {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };
            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);

                var insertOperation = TableOperation.Insert(item);
                var tableResult = await cloudTable.ExecuteAsync(insertOperation).ConfigureAwait(false);

                stopwatchService.Stop();

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results);

                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest },
                        {nameof(results) , results},
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";

                if(exception.Message == "Conflict")
                {
                    insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Conflict;
                }
                else
                {
                    insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;
                }

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }
        public async Task<IEnumerable<TableResult<T>>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.InsertBulkAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Insert Bulk {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };
            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var tableBatchOperation = new TableBatchOperation();
                
                foreach (T item in items)
                {
                    tableBatchOperation.Add(TableOperation.Insert(item));
                }
                var tableResults = await cloudTable.ExecuteBatchAsync(tableBatchOperation).ConfigureAwait(false);
                stopwatchService.Stop();

                var results = tableResults.Select
                (
                    tableResult =>
                    new TableResult<T>
                    {
                        ActivityId = tableResult.ActivityId,
                        Etag = tableResult.Etag,
                        HttpStatusCode = tableResult.HttpStatusCode,
                        RequestCharge = tableResult.RequestCharge,
                        Result = (T)tableResult.Result,
                        SessionToken = tableResult.SessionToken
                    }
                );

                UpdateTelemetryRequest(insertTelemetryRequest, results);

                return results;

            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }

        public async Task<TableResult<T>> UpsertAsync<T>(T item, string tableName) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.UpsertAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Upsert {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var insertOrReplaceOperation = TableOperation.InsertOrReplace(item);
                var tableResult = await cloudTable.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);
                stopwatchService.Stop();

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results);

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }
        public async Task<IEnumerable<TableBatchResult>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity, new()
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.DeleteAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"UpsertBulk Query {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                TableContinuationToken continuationToken = null;
                var results = new List<TableBatchResult>();
                do
                {
                    var cloudTable = _cloudTableClient.GetTableReference(tableName);

                    // Split into chunks of 100 for batching
                    List<List<T>> rowsChunked = items.Select((x, index) => new { Index = index, Value = x })
                        .Where(x => x.Value != null)
                        .GroupBy(x => x.Index / 100)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();

                    // Delete each chunk of 100 in a batch
                    foreach (List<T> rows in rowsChunked)
                    {
                        TableBatchOperation tableBatchOperation = new TableBatchOperation();
                        rows.ForEach(x => tableBatchOperation.Add(TableOperation.InsertOrReplace(x)));

                        results.Add(await cloudTable.ExecuteBatchAsync(tableBatchOperation));
                    }
                }
                while (continuationToken != null);

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }
      
        public async Task<IEnumerable<T>> QueryAsync<T>(string tableName, TableQuery<T> tableQuery) where T : ITableEntity, new()
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.QueryAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Query {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var customerTable = _cloudTableClient.GetTableReference(tableName);
                var results = customerTable.ExecuteQuery(tableQuery);
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                stopwatchService.Stop();

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.SignalResponse = $"Exceptional";
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnHandledException;

                _loggerService.LogErrorRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                throw exception;
            }
            finally
            {
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }

        #region Helpers

        private void UpdateTelemetryRequest<T>(InsertTelemetryRequest insertTelemetryRequest, TableResult<T> tableResult)
        {
            if (tableResult.HttpStatusCode >= 200 && tableResult.HttpStatusCode < 300)
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
            }
            else if (tableResult.HttpStatusCode >= 400 && tableResult.HttpStatusCode < 500)
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.CallerError;
            }
            else
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.ReciverError;
            }

            insertTelemetryRequest.SignalResponse = $"Status Code: {tableResult.HttpStatusCode}";
        }

        private void UpdateTelemetryRequest<T>(InsertTelemetryRequest insertTelemetryRequest, IEnumerable<TableResult<T>> tableResults) where T : ITableEntity
        {
            if (tableResults.All(tableResult => tableResult.HttpStatusCode >= 200 && tableResult.HttpStatusCode < 300))
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
            }
            else if (tableResults.Any(tableResult => tableResult.HttpStatusCode >= 400 && tableResult.HttpStatusCode < 500))
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.CallerError;
            }
            else
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.ReciverError;
            }
        }

        #endregion
    }
    public enum ResultType
    {
        UnhandedException,
        NotExceptional,
    }
}
