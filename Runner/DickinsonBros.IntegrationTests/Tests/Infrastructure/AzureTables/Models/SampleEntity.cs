using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Models
{
    public class SampleEntity : TableEntity
    {
        public string SampleString { get; set; }
    }
}
