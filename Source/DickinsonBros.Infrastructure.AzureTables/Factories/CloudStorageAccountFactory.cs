using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.Infrastructure.AzureTables.Factories
{
    public class CloudStorageAccountFactory : ICloudStorageAccountFactory
    {
        public CloudStorageAccount CreateCloudStorageAccount(string connectionString)
        {
            return CloudStorageAccount.Parse(connectionString);
        }
    }
}
