using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.Cosmos
{
    [ExcludeFromCodeCoverage]
    public class CosmosFactory : ICosmosFactory
    {
        public CosmosClient CreateCosmosClient(CosmosServiceOptions cosmosServiceOptions)
        {
            return new CosmosClient
            (
                cosmosServiceOptions.ConnectionString,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                }
            );
        }

        public Container GetContainer(CosmosClient cosmosClient, CosmosServiceOptions options)
        {
            return cosmosClient.GetContainer(options.DatabaseId, options.ContainerId);
        }
    }
}
