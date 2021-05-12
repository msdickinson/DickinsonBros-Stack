using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.AzureTables.Models
{
    [ExcludeFromCodeCoverage]
    public class AzureTableServiceOptions<T> : AzureTableServiceOptions
    where T : AzureTableServiceOptionsType
    {
    }
}
