using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Abstractions
{
    public interface IAzureTableService<U>
    where U : AzureTableServiceOptionsType
    {
        public Task<TableResult<T>> DeleteAsync<T>(T item, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> DeleteBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;

        public Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;

        public Task<TableResult<T>> InsertAsync<T>(T item, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;

        public Task<IEnumerable<T>> QueryAsync<T>(string tableName, TableQuery<T> tableQuery, bool shouldSendTelemetry = true) where T : ITableEntity, new();
       
        public Task<TableResult<T>> UpsertAsync<T>(T item, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;
    }
}
