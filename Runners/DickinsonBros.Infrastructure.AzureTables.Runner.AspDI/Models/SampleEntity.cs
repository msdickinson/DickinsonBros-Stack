using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Models
{
    public class SampleEntity : TableEntity
    {
        public string URL { get; set; }
        public bool Pass { get; set; }
    }
}
