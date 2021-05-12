using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.Infrastructure.AzureTables.Factories
{
    public interface ICloudTableClientFactory
    {
        CloudTableClient CreateCloudTableClient(CloudStorageAccount cloudStorageAccount);
    }
}
