using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Data.SqlClient;

namespace DickinsonBros.Infrastructure.SQL
{
    public class SQLConnectionService<T> : IDbConnectionService<T>
    where T : SQLServiceOptionsType
    {
        internal readonly SQLServiceOptions<T> _sqlServiceOptions;

        public SQLConnectionService
        (
            IOptions<SQLServiceOptions<T>> options
        )
        {
            _sqlServiceOptions = options.Value;
        }

        public DbConnection Create()
        {
            return new SqlConnection(_sqlServiceOptions.ConnectionString);
        }
    }

}
