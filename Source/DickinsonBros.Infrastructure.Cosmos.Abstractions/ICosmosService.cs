using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos.Abstractions
{
    public interface ICosmosService<U>
    where U : CosmosServiceOptionsType
    {
        Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions) where T : CosmosEntity;
        Task<ItemResponse<T>> DeleteAsync<T>(string id, string key) where T : CosmosEntity;
        Task<IEnumerable<ResponseMessage>> DeleteBulkAsync<T>(IEnumerable<T> items) where T : CosmosEntity;
        Task<ItemResponse<T>> FetchAsync<T>(string id, string key) where T : CosmosEntity;
        Task<IEnumerable<ResponseMessage>> InsertBulkAsync<T>(IEnumerable<T> value) where T : CosmosEntity;
        Task<ItemResponse<T>> InsertAsync<T>(T value) where T : CosmosEntity;
        Task<ItemResponse<T>> UpsertAsync<T>(T value) where T : CosmosEntity;
        Task<IEnumerable<ResponseMessage>> UpsertBulkAsync<T>(IEnumerable<T> items) where T : CosmosEntity;
    }
}
