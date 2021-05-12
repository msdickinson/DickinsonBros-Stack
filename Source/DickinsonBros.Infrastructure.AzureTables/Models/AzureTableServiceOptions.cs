using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.AzureTables.Models
{
    [ExcludeFromCodeCoverage]
    public class AzureTableServiceOptions
    {
        public string ConnectionString { get; set; }
    }
}
