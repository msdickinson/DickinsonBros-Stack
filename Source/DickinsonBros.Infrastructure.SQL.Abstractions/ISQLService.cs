using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL.Abstractions
{
    public interface ISQLService<U>
    where U : SQLServiceOptionsType
    {
        Task BulkCopyAsync(DataTable table, string tableName, int? batchSize = null, TimeSpan? timeout = null, CancellationToken? token = null);
        Task BulkCopyAsync<T>(IEnumerable<T> enumerable, string tableName, int? batchSize = null, TimeSpan? timeout = null, CancellationToken? token = null);
        Task ExecuteAsync(string sql, CommandType commandType, object param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, CommandType commandType, object param = null);
        Task<T> QueryFirstAsync<T>(string sql, CommandType commandType, object param = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, CommandType commandType, object param = null);
    }
}
