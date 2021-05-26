using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Models
{
    public class SampleModel : CosmosEntity
    {
        public string SampleData { get; set; }
    }
}
