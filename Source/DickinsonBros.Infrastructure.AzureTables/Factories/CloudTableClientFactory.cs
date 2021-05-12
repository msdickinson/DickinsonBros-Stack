using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.Infrastructure.AzureTables.Factories
{
    public class CloudTableClientFactory : ICloudTableClientFactory
    {
        public CloudTableClient CreateCloudTableClient(CloudStorageAccount cloudStorageAccount)
        {
            return cloudStorageAccount.CreateCloudTableClient();
        }
    }
}
