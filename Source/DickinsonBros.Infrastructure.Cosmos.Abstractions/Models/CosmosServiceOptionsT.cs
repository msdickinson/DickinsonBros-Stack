using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class CosmosServiceOptions<T> : CosmosServiceOptions
    where T : CosmosServiceOptionsType
    {

    }
}
