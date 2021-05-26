using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SQL.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class SQLServiceOptions<T> : SQLServiceOptions
    where T : SQLServiceOptionsType
    {
        public string ConnectionName { get; set; }
    }
}
