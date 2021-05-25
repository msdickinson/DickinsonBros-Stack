using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class CosmosServiceOptions
    {
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
        public string ConnectionString { get; set; }
    }
}
