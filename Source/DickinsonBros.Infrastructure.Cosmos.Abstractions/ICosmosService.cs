using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions
{
    public interface ICosmosService<U>
    {
        Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions);
        Task<ItemResponse<T>> DeleteAsync<T>(string id, string key);
        Task<IEnumerable<ResponseMessage>> DeleteBulkAsync<T>(IEnumerable<string> ids, string key);
        Task<ItemResponse<T>> FetchAsync<T>(string id, string key);
        Task<IEnumerable<ResponseMessage>> InsertBulkAsync<T>(IEnumerable<T> value, string key);
        Task<ItemResponse<T>> InsertAsync<T>(T value, string key);
        Task<ItemResponse<T>> UpsertAsync<T>(T value, string key, string eTag);
    }
}
