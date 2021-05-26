using System.Collections.Generic;
using System.Data;

namespace DickinsonBros.Infrastructure.SQL.Abstractions
{
    public interface IDataTableService
    {
        DataTable ToDataTable<T>(IEnumerable<T> enumerable, string tableName);
    }
}
