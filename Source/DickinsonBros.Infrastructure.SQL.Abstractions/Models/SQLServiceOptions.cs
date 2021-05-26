using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SQL.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class SQLServiceOptions
    {
        public string ConnectionString { get; set; }
    }
}
