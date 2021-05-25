using DickinsonBros.Infrastructure.Cosmos.Models;

namespace DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Models
{
    public class SampleModel : CosmosEntity
    {
        public string SampleData { get; set; }
    }
}
