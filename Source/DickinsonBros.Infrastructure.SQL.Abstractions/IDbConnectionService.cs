using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using System.Data.Common;

namespace DickinsonBros.Infrastructure.SQL.Abstractions
{
    public interface IDbConnectionService<T>
    where T : SQLServiceOptionsType
    {
        DbConnection Create();
    }
}
