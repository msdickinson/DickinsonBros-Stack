using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.AzureTables.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TableResult<T>
    {
        public T Result { get; set; }
        public int HttpStatusCode { get; set; }
        public string Etag { get; set; }
        public string SessionToken { get; set; }
        public double? RequestCharge { get; set; }
        public string ActivityId { get; set; }
    }
}
