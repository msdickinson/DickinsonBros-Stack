using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using Microsoft.Azure.Cosmos;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions
{
    public interface ICosmosFactory
    {
        CosmosClient CreateCosmosClient(CosmosServiceOptions cosmosServiceOptions);
        Container GetContainer(CosmosClient cosmosClient, CosmosServiceOptions value);
    }
}
