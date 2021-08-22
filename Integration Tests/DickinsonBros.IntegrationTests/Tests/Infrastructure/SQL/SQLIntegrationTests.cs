using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.IntegrationTests.Tests.Infrastructure.SQL.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.SQL
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "SQL", Group = "Infrastructure")]
    public class SQLIntegrationTests : ISQLIntegrationTests
    {
        public readonly ITelemetryWriterService _telemetryWriterService;
        public readonly ISQLService<RunnerSQLServiceOptionsType> _sqlService;

        internal const string KEY = "DickinsonBrosIntegrationTests";
        internal const string KEYBULK = "DickinsonBrosIntegrationTestsBulk";

        public SQLIntegrationTests
        (
            ITelemetryWriterService telemetryWriterService,
            ISQLService<RunnerSQLServiceOptionsType> sqlService
        )
        {
            _sqlService = sqlService;
            _telemetryWriterService = telemetryWriterService;
        }

        public async Task ExecuteAndQueryAndBulkInsert_Runs_ExpectedReturnsAndNoThrows(List<string> successLog)
        {
            _telemetryWriterService.ScopedUserStory = "SQL";

            var sampleEntity = new SampleEntity
            {
                Payload = Guid.NewGuid().ToString()
            };

            //Execute (Delete) 
            await _sqlService.ExecuteAsync("DELETE FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);

            //Execute Insert
            await _sqlService.ExecuteAsync(
@"INSERT INTO [dbo].[Samples]
        ([Payload])
    VALUES
        (@Payload);",
                            CommandType.Text,
                            sampleEntity
                        ).ConfigureAwait(false);
            successLog.Add($"ExecuteAsync Successful. Inserted SampleEntity");

            //Query First
            var queryFirstResponse =
                await _sqlService.QueryFirstAsync<SampleEntity>
                (
                    "SELECT top(1) * FROM [dbo].[Samples] where Payload = @Payload",
                    CommandType.Text,
                    new
                    {
                        Payload = sampleEntity.Payload
                    }
               ).ConfigureAwait(false);

            Assert.AreEqual(sampleEntity.Payload, queryFirstResponse.Payload, "QueryFirstAsync Failed");
            successLog.Add($"QueryFirstAsync Successful.");

            //Execute Update
            await _sqlService.ExecuteAsync
            (
@"UPDATE .[dbo].[Samples]
SET [Payload] = @Payload
WHERE [SamplesId] = @SamplesId",
                    CommandType.Text,
                    new
                    {
                        Payload = queryFirstResponse.Payload + " Edited",
                        SamplesId = queryFirstResponse.SamplesId
                    }
           ).ConfigureAwait(false);
            successLog.Add($"ExecuteAsync Successful.");

            //Query First Or Default
            var queryFirstOrDefaultResponse =
                await _sqlService.QueryFirstOrDefaultAsync<SampleEntity>
                (
                    "SELECT top(1) * FROM [dbo].[Samples] where Payload = @Payload",
                    CommandType.Text,
                    new
                    {
                        Payload = sampleEntity.Payload + " Edited"
                    }
               ).ConfigureAwait(false);

            Assert.AreEqual(queryFirstResponse.SamplesId, queryFirstOrDefaultResponse.SamplesId, "QueryFirstOrDefaultAsync Failed");
            successLog.Add($"QueryFirstOrDefaultAsync Successful.");

            //Query
            var queryResponse = await _sqlService.QueryAsync<SampleEntity>("SELECT * FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);
            Assert.IsTrue(queryResponse.Any(), "QueryAsync Failed");
            successLog.Add($"QueryAsync Successful.");

            //Bulk Insert
            await _sqlService.BulkCopyAsync(queryResponse, "[dbo].[Samples]").ConfigureAwait(false);
            successLog.Add($"BulkCopyAsync Successful.");

        }

    }
}
