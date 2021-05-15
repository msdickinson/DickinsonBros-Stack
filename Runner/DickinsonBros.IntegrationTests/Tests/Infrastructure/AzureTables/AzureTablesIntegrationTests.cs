using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.AzureTables
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "AzureTables", Group = "Infrastructure")]
    public class AzureTablesIntegrationTests : IAzureTablesIntegrationTests
    {
        public IAzureTableService<RunnerAzureTableServiceOptionsType> _azureTableService;

        internal const string TABLE_NAME = "DickinsonBros-IntegrationTests-AzureTables";

        public AzureTablesIntegrationTests
        (
            IAzureTableService<RunnerAzureTableServiceOptionsType> azureTableService
        )
        {
            _azureTableService = azureTableService;
        }

        public async Task InsertAndFetch_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity();

            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. RowKey: {sampleEntity.RowKey}");
            
            var fetchAsyncResult = await _azureTableService.FetchAsync<SampleEntity>(sampleEntity.PartitionKey, sampleEntity.RowKey, sampleTableName).ConfigureAwait(false);
            successLog.Add($"Fetch Successful. RowKey: {sampleEntity.RowKey}");
        }

        public async Task Upsert_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity();

            //Upsert
            var upsertAsyncResult = await _azureTableService.UpsertAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, upsertAsyncResult.HttpStatusCode, "Upsert Failed");
            successLog.Add($"Upsert Successful. RowKey: {sampleEntity.RowKey}");
        }

        public async Task InsertAndUpsert_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity();

            //Insert
            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. RowKey: {sampleEntity.RowKey}");

            //Upsert
            sampleEntity.SampleString = Guid.NewGuid().ToString();

            var upsertResult = await _azureTableService.UpsertAsync(insertAsyncResult.Result, TABLE_NAME).ConfigureAwait(false);

            Assert.IsNotNull(upsertResult.Result, "Fetch Failed");
            Assert.AreEqual(urlB, upsertResult.Result?.URL, "Inserted URL does not match Fetched URL");
            successLog.Add($"Upsert Successful");
        }

        public async Task InsertAndDelete_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity("InsertDelete");
            var sampleTableName = "SampleTable";

            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, sampleTableName).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. ETag: { insertAsyncResult.Etag }, RowKey: {sampleEntity.RowKey}");

            var fetchAsyncResult = await _azureTableService.DeleteAsync(sampleEntity, sampleTableName).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Delete Failed");
            successLog.Add($"Delete Successful");
        }

        public async Task InsertAndQueryAsync_Runs_ValuesMatch(List<string> successLog)
        {
            var URL = Guid.NewGuid().ToString();
            var sampleEntity = GenerateNewSampleEntity(URL);
            var sampleTableName = "SampleTable";

            //Insert
            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, sampleTableName).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. ETag: { insertAsyncResult.Etag }, RowKey: {sampleEntity.RowKey}");

            //Query
            var tableQuery = new TableQuery<SampleEntity>()
                            .Where
                            (
                                TableQuery.GenerateFilterCondition("URL", QueryComparisons.Equal, URL)
                            );

            var queryAsyncResult = (await _azureTableService.QueryAsync(sampleTableName, tableQuery).ConfigureAwait(false)).ToList();
            Assert.AreEqual(1, queryAsyncResult.Count());
        }

        public async Task InsertAndBulkDelete_Runs_IsSuccessful(List<string> successLog)
        {
            var sampleTableName = "SampleTable";
            var sampleEntitys = new List<SampleEntity>();

            for (int i = 0; i < 2; i++)
            {
                sampleEntitys.Add(GenerateNewSampleEntity("BulkInsert"));
            }

            var insertBulkAsyncResult = await _azureTableService.InsertBulkAsync(sampleEntitys, sampleTableName, true).ConfigureAwait(false);
            var deleteAsyncResult = await _azureTableService.DeleteBulkAsync(insertBulkAsyncResult.Select(e=> e.Result), sampleTableName).ConfigureAwait(false);

            Assert.AreEqual(2, insertBulkAsyncResult.Count());
        }

        public async Task InsertBulkAndUpsertBulkAsync_Runs_IsSuccessful(List<string> successLog)
        {
            var sampleTableName = "SampleTable";
            var sampleEntitys = new List<SampleEntity>();
            var urlA = Guid.NewGuid().ToString();
            var urlB = Guid.NewGuid().ToString();
            var urlC = Guid.NewGuid().ToString();

            for (int i = 0; i < 2; i++)
            {
                sampleEntitys.Add(GenerateNewSampleEntity(urlA));
            }

            var insertBulkAsyncResult = await _azureTableService.InsertBulkAsync(sampleEntitys, sampleTableName, true).ConfigureAwait(false);

            Assert.AreEqual(2, insertBulkAsyncResult.Count());

            var sampleEntitysPlusOne = new List<SampleEntity>();
            sampleEntitysPlusOne.AddRange(insertBulkAsyncResult.Select(e=> e.Result));
            sampleEntitysPlusOne.ForEach((e) => { e.URL = urlB; });
            sampleEntitysPlusOne.Add(GenerateNewSampleEntity(urlC));

            var upsertBulkAsyncResult = await _azureTableService.UpsertBulkAsync(sampleEntitysPlusOne, sampleTableName).ConfigureAwait(false);

            Assert.AreEqual(3, upsertBulkAsyncResult.Count());
        }

   
        private SampleEntity GenerateNewSampleEntity()
        {
            var sampleEntity = new SampleEntity();
            sampleEntity.SampleString = Guid.NewGuid().ToString();
            sampleEntity.PartitionKey = System.DateTime.UtcNow.ToShortDateString();
            sampleEntity.RowKey = Guid.NewGuid().ToString();
            sampleEntity.PartitionKey = "PartitionKey";
            sampleEntity.Timestamp = DateTime.UtcNow;
            return sampleEntity;
        }
    }
}
