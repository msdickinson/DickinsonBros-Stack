using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Abstractions
{
    public interface IAzureTableService<U>
    where U : AzureTableServiceOptionsType
    {
        //Build/Automation - In Progress
        //Unit Tested

        //Delete [2/2]
        public Task<TableResult<T>> DeleteAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> DeleteBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity;

        //Fetch [1/1]
        public Task<TableResult<T>> FetchAsync<T>(string partitionKey, string rowkey, string tableName) where T : ITableEntity;

        //Insert [2/2]
        public Task<TableResult<T>> InsertAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> InsertBulkAsync<T>(IEnumerable<T> items, string tableName, bool shouldSendTelemetry = true) where T : ITableEntity;

        //Query [1/1]
        public Task<IEnumerable<T>> QueryAsync<T>(string tableName, TableQuery<T> tableQuery) where T : ITableEntity, new();
       
        //Upsert [2/2]
        public Task<TableResult<T>> UpsertAsync<T>(T item, string tableName) where T : ITableEntity;
        public Task<IEnumerable<TableBatchResult>> UpsertBulkAsync<T>(IEnumerable<T> items, string tableName) where T : ITableEntity, new();
    }
}
