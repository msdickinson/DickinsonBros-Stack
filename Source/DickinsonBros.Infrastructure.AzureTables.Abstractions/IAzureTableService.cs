using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Abstractions
{
    public interface IAzureTableService<U>
    where U : AzureTableServiceOptionsType
    {
        public Task<TableResult<T>> DeleteAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName) where T : ITableEntity;
        public Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, EntityResolver<T> entityResolver, string tableName) where T : ITableEntity;
        public Task<TableResult<T>> InsertAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<IEnumerable<TableResult<T>>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;
        public Task<TableResult<T>> UpsertAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<IEnumerable<TableResult<T>>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity;
    }
}
