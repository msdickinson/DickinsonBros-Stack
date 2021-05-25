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
using DickinsonBros.Core.Correlation.Abstractions;

namespace DickinsonBros.Infrastructure.AzureTables
{
    public class AzureTableService<U> : IAzureTableService<U>
    where U : AzureTableServiceOptionsType
    {
        internal readonly ILoggerService<U> _loggerService;
        internal readonly ICorrelationService _correlationService;
        internal readonly CloudStorageAccount _cloudStorageAccount;
        internal readonly CloudTableClient _cloudTableClient;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ITelemetryWriterService _telemetryWriterService;

        public AzureTableService
        (
            ILoggerService<U> loggerService, 
            ICorrelationService correlationService,
            ICloudStorageAccountFactory cloudStorageAccountFactory,
            ICloudTableClientFactory cloudTableClientFactory,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory, 
            ITelemetryWriterService telemetryWriterService,
            IOptions<AzureTableServiceOptions<U>> options
        )
        {
            _loggerService = loggerService;
            _correlationService = correlationService;
            _cloudStorageAccount = cloudStorageAccountFactory.CreateCloudStorageAccount(options.Value.ConnectionString);
            _cloudTableClient = cloudTableClientFactory.CreateCloudTableClient(_cloudStorageAccount);
            _dateTimeService = dateTimeService;
            _stopwatchFactory = stopwatchFactory;
            _telemetryWriterService = telemetryWriterService;
        }

        public async Task<TableResult<T>> DeleteAsync<T>(T item, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.DeleteAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Delete {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var deleteOperation = TableOperation.Delete(item);
                var tableResult = await cloudTable.ExecuteAsync(deleteOperation).ConfigureAwait(false);
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results.HttpStatusCode);

                //Log
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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

                throw;
            }
            finally
            {
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }
    
        public async Task<IEnumerable<TableBatchResult>> DeleteBulkAsync<T>(IEnumerable<T> items, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.DeleteBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"DeleteBulk {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var results = new List<TableBatchResult>();
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

                //Log
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }

        }
    
        public async Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.FetchAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Fetch {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowkey);
                var tableResult = await cloudTable.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results.HttpStatusCode);
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }
       
        public async Task<TableResult<T>> InsertAsync<T>(T item, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.InsertAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Insert {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);

                var insertOperation = TableOperation.Insert(item);
                var tableResult = await cloudTable.ExecuteAsync(insertOperation).ConfigureAwait(false);
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results.HttpStatusCode);
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        {nameof(insertTelemetryRequest) , insertTelemetryRequest },
                        {nameof(results) , results}
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                if(exception.Message == "Conflict")
                {
                    insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Conflict;
                }
                else
                {
                    insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }
     
        public async Task<IEnumerable<TableBatchResult>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.InsertBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"InsertBulk {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var results = new List<TableBatchResult>();
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
                    rows.ForEach(x => tableBatchOperation.Add(TableOperation.Insert(x)));

                    results.Add(await cloudTable.ExecuteBatchAsync(tableBatchOperation));
                }

                //Log
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if(sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }
       
        public async Task<IEnumerable<T>> QueryAsync<T>(string tableName, TableQuery<T> tableQuery, bool sendTelemetry = true) where T : ITableEntity, new()
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.QueryAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Query {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var results = new List<T>();
                TableContinuationToken token = null;
                var customerTable = _cloudTableClient.GetTableReference(tableName);
                do
                {
                    TableQuerySegment<T> seg = await customerTable.ExecuteQuerySegmentedAsync<T>(tableQuery, token);
                    token = seg.ContinuationToken;
                    results.AddRange(seg);
                } while (token != null);

                //Log
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                            {nameof(insertTelemetryRequest) , insertTelemetryRequest },
                            {nameof(results) , results}
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }

        public async Task<TableResult<T>> UpsertAsync<T>(T item, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.UpsertAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Upsert {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var insertOrReplaceOperation = TableOperation.InsertOrReplace(item);
                var tableResult = await cloudTable.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpdateTelemetryRequest(insertTelemetryRequest, results.HttpStatusCode);
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }
        }

        public async Task<IEnumerable<TableBatchResult>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool sendTelemetry = true) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{typeof(U).Name}>.{nameof(IAzureTableService<U>.UpsertBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cloudStorageAccount.TableStorageUri.PrimaryUri.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"UpsertBulk {typeof(T).Name}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var results = new List<TableBatchResult>();
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

                //Log
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                _loggerService.LogInformationRedacted
                (
                    methodName,
                    Core.Logger.Abstractions.Models.LogGroup.Infrastructure,
                    new Dictionary<string, object>()
                    {
                        { nameof(insertTelemetryRequest) , insertTelemetryRequest }
                    }
                );

                return results;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

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
                if (sendTelemetry)
                {
                    _telemetryWriterService.Insert(insertTelemetryRequest);
                }
            }

        }


        #region Helpers

        internal void UpdateTelemetryRequest(InsertTelemetryItem insertTelemetryRequest, int httpStatusCode)
        {
            if (httpStatusCode >= 200 && httpStatusCode < 300)
            {
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
            }
            else if (httpStatusCode >= 400 && httpStatusCode < 500)
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
