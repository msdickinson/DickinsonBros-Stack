using Microsoft.Azure.Cosmos.Table;

namespace DickinsonBros.Infrastructure.AzureTables.Factories
{
    public interface ICloudStorageAccountFactory
    {
        CloudStorageAccount CreateCloudStorageAccount(string connectionString);
    }
}
