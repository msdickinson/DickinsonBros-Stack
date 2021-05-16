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

        internal const string TABLE_NAME = "DickinsonBrosIntegrationTests";

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
            
            var fetchAsyncResult = await _azureTableService.FetchAsync<SampleEntity>(sampleEntity.PartitionKey, sampleEntity.RowKey, TABLE_NAME).ConfigureAwait(false);
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
            successLog.Add($"Insert Successful. RowKey: {insertAsyncResult.Result.RowKey}");

            //Upsert
            sampleEntity.SampleString = Guid.NewGuid().ToString();
            var upsertResult = await _azureTableService.UpsertAsync(insertAsyncResult.Result, TABLE_NAME).ConfigureAwait(false);
            Assert.IsNotNull(upsertResult.Result, "Upsert Failed");
            successLog.Add($"Upsert Successful. RowKey: {upsertResult.Result.RowKey}");
        }

        public async Task InsertAndDelete_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity();

            //Insert
            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. RowKey: {insertAsyncResult.Result.RowKey}");

            //Delete
            var deleteAsyncResult = await _azureTableService.DeleteAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, deleteAsyncResult.HttpStatusCode, "Delete Failed");
            successLog.Add($"Delete Successful");
        }

        public async Task InsertAndQueryAsync_Runs_ValuesMatch(List<string> successLog)
        {
            var sampleEntity = GenerateNewSampleEntity();

            //Insert
            var insertAsyncResult = await _azureTableService.InsertAsync(sampleEntity, TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(204, insertAsyncResult.HttpStatusCode, "Insert Failed");
            successLog.Add($"Insert Successful. RowKey: {insertAsyncResult.Result.RowKey}");

            //Query
            var tableQuery = new TableQuery<SampleEntity>()
                            .Where
                            (
                                TableQuery.GenerateFilterCondition(nameof(SampleEntity.SampleString), QueryComparisons.Equal, sampleEntity.SampleString)
                            );

            var queryAsyncResult = (await _azureTableService.QueryAsync(TABLE_NAME, tableQuery).ConfigureAwait(false)).ToList();
            Assert.AreEqual(1, queryAsyncResult.Count(), "Query Failed");
            successLog.Add($"Query Successful. RowKey: {queryAsyncResult.First().RowKey}");
        }

        public async Task InsertBulkAndBulkDelete_Runs_IsSuccessful(List<string> successLog)
        {
            var sampleEntitys = new List<SampleEntity>();
            for (int i = 0; i < 2; i++)
            {
                sampleEntitys.Add(GenerateNewSampleEntity());
            }

            //Insert Bulk
            var insertBulkAsyncResult = await _azureTableService.InsertBulkAsync(sampleEntitys, TABLE_NAME, true).ConfigureAwait(false);
            Assert.AreEqual(1, insertBulkAsyncResult.Count(), "Insert Bulk Failed");
            successLog.Add($"Insert Bulk Successful.");

            //Delete Bulk
            var deleteAsyncResult = await _azureTableService.DeleteBulkAsync(insertBulkAsyncResult.First().Select(e => (SampleEntity)e.Result), TABLE_NAME).ConfigureAwait(false);
            Assert.AreEqual(2, deleteAsyncResult.FirstOrDefault().Count(), "Delete Bulk Failed");
            successLog.Add($"Delete Bulk Successful");
        }

        public async Task InsertBulkAndUpsertBulkAsync_Runs_IsSuccessful(List<string> successLog)
        {
            var sampleEntitys = new List<SampleEntity>();

            for (int i = 0; i < 2; i++)
            {
                sampleEntitys.Add(GenerateNewSampleEntity());
            }

            //Insert Bulk
            var insertBulkAsyncResult = await _azureTableService.InsertBulkAsync(sampleEntitys, TABLE_NAME, true).ConfigureAwait(false);
            Assert.AreEqual(2, insertBulkAsyncResult.First().Count(), "Insert Bulk Failed");
            successLog.Add($"Insert Bulk Successful");

            //Upsert Bulk
            var sampleEntitysPlusOne = new List<SampleEntity>();
            var sampleStringReplace = Guid.NewGuid().ToString();

            sampleEntitysPlusOne.AddRange(insertBulkAsyncResult.First().Select(e=> (SampleEntity)e.Result));
            sampleEntitysPlusOne.ForEach((e) => { e.SampleString = sampleStringReplace; });
            sampleEntitysPlusOne.Add(GenerateNewSampleEntity());

            var upsertBulkAsyncResult = await _azureTableService.UpsertBulkAsync(sampleEntitysPlusOne, TABLE_NAME).ConfigureAwait(false);

            Assert.AreEqual(3, upsertBulkAsyncResult.First().Count(), "Upsert Bulk Failed");
            successLog.Add($"Upsert Bulk Successful");
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
