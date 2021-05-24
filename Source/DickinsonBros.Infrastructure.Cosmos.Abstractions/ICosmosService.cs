using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions
{
    public interface ICosmosService<U>
    {
        Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions);
        Task<ItemResponse<T>> DeleteAsync<T>(string id, string key);
        Task<ItemResponse<T>> FetchAsync<T>(string id, string key);
        Task<IEnumerable<ResponseMessage>> BulkInsertAsync<T>(string key, IEnumerable<T> value);
        Task<ItemResponse<T>> InsertAsync<T>(string key, T value);
        Task<ItemResponse<T>> UpsertAsync<T>(string key, string eTag, T value);
    }
}
