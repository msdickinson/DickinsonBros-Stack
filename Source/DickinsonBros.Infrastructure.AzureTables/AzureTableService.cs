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
            IOptions<AzureTableServiceOptions<U>> options
        )
        {
            _loggerService = loggerService;
            _cloudStorageAccount = cloudStorageAccountFactory.CreateCloudStorageAccount(options.Value.ConnectionString);
            _cloudTableClient = cloudTableClientFactory.CreateCloudTableClient(_cloudStorageAccount);
        }

        #region InsertAsync
        public async Task<TableResult<T>> InsertAsync<T>(T item, string tableName) where T : ITableEntity
        {
            return await InsertAsync(item, tableName, null).ConfigureAwait(false);
        }

        public async Task<TableResult<T>> InsertAsync<T>(T item, string tableName, Action<InsertTelemetryRequest, TableResult<T>> postCallMethod) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.InsertAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Insert {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };
            var dateTimeService = _stopwatchFactory.NewStopwatchService();
            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);

                var insertOperation = TableOperation.Insert(item);
                var tableResult = await cloudTable.ExecuteAsync(insertOperation).ConfigureAwait(false);

                dateTimeService.Stop();

                var results = new TableResult<T>
                {
                    ActivityId = tableResult.ActivityId,
                    Etag = tableResult.Etag,
                    HttpStatusCode = tableResult.HttpStatusCode,
                    RequestCharge = tableResult.RequestCharge,
                    Result = (T)tableResult.Result,
                    SessionToken = tableResult.SessionToken
                };

                UpateInsertTelemetryRequest(insertTelemetryRequest, results);
                postCallMethod?.Invoke(insertTelemetryRequest, results);

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
                dateTimeService.Stop();
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
                insertTelemetryRequest.Duration = dateTimeService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }

        #endregion

        #region InsertBulkAsync

        public async Task<IEnumerable<TableResult<T>>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity
        {
            return await InsertBulkAsync(items, tableName, null).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TableResult<T>>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, Action<InsertTelemetryRequest, IEnumerable<TableResult<T>>> postCallMethod) where T : ITableEntity
        {
            var methodName = $"{nameof(IAzureTableService<U>)}<{nameof(U)}>.{nameof(IAzureTableService<U>.InsertBulkAsync)}<{nameof(T)}>";

            var insertTelemetryRequest = new InsertTelemetryRequest
            {
                ConnectionName = $"{_cloudStorageAccount.TableStorageUri} {_cloudTableClient.BaseUri} {_cloudTableClient.StorageUri}",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                SignalRequest = $"Bulk Insert {nameof(T)}. Tablename: {tableName}",
                TelemetryType = TelemetryType.AzureTable
            };
            var dateTimeService = _stopwatchFactory.NewStopwatchService();

            try
            {
                var cloudTable = _cloudTableClient.GetTableReference(tableName);
                var tableBatchOperation = new TableBatchOperation();
                foreach (T item in items)
                {
                    tableBatchOperation.Add(TableOperation.Insert(item));
                }
                var tableResults = await cloudTable.ExecuteBatchAsync(tableBatchOperation).ConfigureAwait(false);

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

                UpateBulkInsertTelemetryRequest(insertTelemetryRequest, results);
                postCallMethod?.Invoke(insertTelemetryRequest, results);

                return results;

            }
            catch (Exception exception)
            {
                dateTimeService.Stop();
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
                insertTelemetryRequest.Duration = dateTimeService.Elapsed;
                await _telemetryServiceWriter.InsertAsync(insertTelemetryRequest).ConfigureAwait(false);
            }
        }

        #endregion

        public async Task<TableResult<T>> UpsertAsync<T>(T item, string tableName) where T : ITableEntity
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(item);
            var tableResult = await cloudTable.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);

            return new TableResult<T>
            {
                ActivityId = tableResult.ActivityId,
                Etag = tableResult.Etag,
                HttpStatusCode = tableResult.HttpStatusCode,
                RequestCharge = tableResult.RequestCharge,
                Result = (T)tableResult.Result,
                SessionToken = tableResult.SessionToken
            };

            //Push Event.
        }

        public async Task<IEnumerable<TableResult<T>>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            var tableBatchOperation = new TableBatchOperation();
            foreach (T item in items)
            {
                tableBatchOperation.Add(TableOperation.InsertOrReplace(item));
            }
            var tableResults = await cloudTable.ExecuteBatchAsync(tableBatchOperation).ConfigureAwait(false);

            return tableResults.Select
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
        }

        public async Task<TableResult<T>> DeleteAsync<T>(T item, string tableName) where T : ITableEntity
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            var deleteOperation = TableOperation.Delete(item);
            var tableResult = await cloudTable.ExecuteAsync(deleteOperation).ConfigureAwait(false);

            return new TableResult<T>
            {
                ActivityId = tableResult.ActivityId,
                Etag = tableResult.Etag,
                HttpStatusCode = tableResult.HttpStatusCode,
                RequestCharge = tableResult.RequestCharge,
                Result = (T)tableResult.Result,
                SessionToken = tableResult.SessionToken
            };
        }

        public async Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName) where T : ITableEntity
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowkey);
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            return new TableResult<T>
            {
                ActivityId = tableResult.ActivityId,
                Etag = tableResult.Etag,
                HttpStatusCode = tableResult.HttpStatusCode,
                RequestCharge = tableResult.RequestCharge,
                Result = (T)tableResult.Result,
                SessionToken = tableResult.SessionToken
            };
        }

        public async Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, EntityResolver<T> entityResolver, string tableName) where T : ITableEntity
        {
            var cloudTable = _cloudTableClient.GetTableReference(tableName);
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowkey, entityResolver);
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            return new TableResult<T>
            {
                ActivityId = tableResult.ActivityId,
                Etag = tableResult.Etag,
                HttpStatusCode = tableResult.HttpStatusCode,
                RequestCharge = tableResult.RequestCharge,
                Result = (T)tableResult.Result,
                SessionToken = tableResult.SessionToken
            };
        }

        #region Update Telemetry
        private void UpateInsertTelemetryRequest<T>(InsertTelemetryRequest insertTelemetryRequest, TableResult<T> tableResult)
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

        private void UpateBulkInsertTelemetryRequest<T>(InsertTelemetryRequest insertTelemetryRequest, IEnumerable<TableResult<T>> tableResults) where T : ITableEntity
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

            //insertTelemetryRequest.SignalResponse = $"Status Code: {tableResults.HttpStatusCode}";
        }
        #endregion



    }
    public enum ResultType
    {
        UnhandedException,
        NotExceptional,
    }
}
