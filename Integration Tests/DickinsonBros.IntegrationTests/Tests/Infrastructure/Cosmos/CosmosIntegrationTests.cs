﻿using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Cosmos", Group = "Infrastructure")]
    public class CosmosIntegrationTests : ICosmosIntegrationTests
    {
        public readonly ICosmosService<RunnerCosmosServiceOptionsType> _cosmosService;

        internal const string KEY = "DickinsonBrosIntegrationTests";
        internal const string KEYBULK = "DickinsonBrosIntegrationTestsBulk";

        public CosmosIntegrationTests
        (
            ICosmosService<RunnerCosmosServiceOptionsType> cosmosService
        )
        {
            _cosmosService = cosmosService;
        }

        public async Task InsertAndUpsertAndFetchAndDelete_Runs_ExpectedStatusCodes(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleModel();

            var insertAsyncResult = await _cosmosService.InsertAsync(sampleEntity, KEY).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.Created, insertAsyncResult.StatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. Id: {sampleEntity.Id}");

            insertAsyncResult.Resource.SampleData = "Changed Value";
            var upsertAsyncResult = await _cosmosService.UpsertAsync(insertAsyncResult.Resource, KEY, insertAsyncResult.Resource._etag).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.Created, insertAsyncResult.StatusCode, "Upsert Failed");
            successLog.Add($"Upsert Successful. Id: {upsertAsyncResult.Resource.Id}");

            var fetchAsyncResult = await _cosmosService.FetchAsync<SampleModel>(upsertAsyncResult.Resource.Id, KEY).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, fetchAsyncResult.StatusCode, $"Fetch Successful. Status Code: {fetchAsyncResult.StatusCode}");
            successLog.Add($"Fetch Successful. Id: {fetchAsyncResult.Resource.Id}");

            var deleteAsyncResult = await _cosmosService.DeleteAsync<SampleModel>(fetchAsyncResult.Resource.Id, KEY).ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, fetchAsyncResult.StatusCode, $"Fetch Successful. Status Code: {fetchAsyncResult.StatusCode}");
            successLog.Add($"Fetch Successful. Id: {fetchAsyncResult.Resource.Id}");
        }


        public async Task InsertBulkAndQueryAndDeleteBulk_Runs_ValuesMatch(List<string> successLog)
        {
            //Bulk Insert
            var sampleModelValues = new List<SampleModel>();
            for (var i = 0; i < 3; i++)
            {
                sampleModelValues.Add(GenerateNewSampleModel("BULK"));
            }

            var bulkInsertResult = await _cosmosService.InsertBulkAsync(sampleModelValues, KEY).ConfigureAwait(false);
            Assert.IsTrue(bulkInsertResult.All(e=> e.StatusCode == HttpStatusCode.Created), "Bulk Insert Failed");
            successLog.Add($"Bulk Insert Successful");

            //Query
            var queryResult = await _cosmosService.QueryAsync<SampleModel>
                               (
                                   new QueryDefinition($"SELECT TOP 3 * FROM c where c.key='{KEY}' and c.sampleData='BULK'"),
                                   new QueryRequestOptions
                                   {
                                       PartitionKey = new PartitionKey(KEY),
                                       MaxItemCount = 3
                                   }
                               ).ConfigureAwait(false);
            Assert.AreEqual(3, queryResult.Count(), "Query Failed");
            successLog.Add($"Query Successful");

            //Bulk Delete
            var deleteBulkResult = await _cosmosService.DeleteBulkAsync<SampleModel>(queryResult.Select(e => e.Id), KEY).ConfigureAwait(false);
            Assert.IsTrue(deleteBulkResult.All(e => e.StatusCode == HttpStatusCode.NoContent), "Delete Insert Failed");
            successLog.Add($"Delete Bulk Successful");
        }

        private SampleModel GenerateNewSampleModel(string sampleData = null)
        {
            var guid = Guid.NewGuid().ToString();
            var value = sampleData ?? Guid.NewGuid().ToString();
            var key = KEY;

            return new SampleModel
            {
                Id = guid,
                Key = key,
                SampleData = value
            };
        }
    }
}
