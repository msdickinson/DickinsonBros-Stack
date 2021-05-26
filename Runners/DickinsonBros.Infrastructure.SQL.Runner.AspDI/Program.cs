using BaseRunner;
using Dickinsonbros.Core.Guid.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Correlation.Adapter.AspDI.Extensions;
using DickinsonBros.Core.DateTime.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Logger.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Redactor.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Stopwatch.Adapter.AspDI.Extensions;
using DickinsonBros.Core.Telemetry.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.AspDI.Extensions;
using DickinsonBros.Infrastructure.AzureTables.Runner.AspDI.Config;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.AspDI.Extensions;
using DickinsonBros.Infrastructure.SQL.Runner.AspDI.Config;
using DickinsonBros.Infrastructure.SQL.Runner.AspDI.Models;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL.Runner.AspDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                using var provider = serviceCollection.BuildServiceProvider();
                var sqlService = provider.GetRequiredService<ISQLService<RunnerSQLServiceOptionsType>>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var sampleEntity = new SampleEntity
                {
                    Payload = Guid.NewGuid().ToString()
                };

                var sampleEntityTwo = new SampleEntity
                {
                    Payload = Guid.NewGuid().ToString()
                };

                //Execute Insert
                await sqlService.ExecuteAsync(
@"INSERT INTO [dbo].[Samples]
        ([Payload])
    VALUES
        (@Payload);",
                CommandType.Text,
                sampleEntity
            ).ConfigureAwait(false);

                await sqlService.ExecuteAsync(
@"INSERT INTO [dbo].[Samples]
        ([Payload])
    VALUES
        (@Payload);",
                                CommandType.Text,
                                sampleEntityTwo
                            ).ConfigureAwait(false);

                //Query First
                var queryFirstResponse = await sqlService.QueryFirstAsync<SampleEntity>("SELECT top(1) * FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);

                queryFirstResponse.Payload += " Edited";
                //Execute Update
                await sqlService.ExecuteAsync(
@"UPDATE .[dbo].[Samples]
SET [Payload] = @Payload
WHERE [SamplesId] = @SamplesId",
                                                CommandType.Text,
                                                queryFirstResponse
                                            ).ConfigureAwait(false);


                //Query First Or Default
                var queryFirstOrDefaultResponse = await sqlService.QueryFirstOrDefaultAsync<SampleEntity>("SELECT top(1) * FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);

                //Query
                var queryResponse = await sqlService.QueryAsync<SampleEntity>("SELECT * FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);

                //Bulk Insert
                await sqlService.BulkCopyAsync(queryResponse, "[dbo].[Samples]").ConfigureAwait(false);

                //Execute (Delete) 
                await sqlService.ExecuteAsync("DELETE FROM [dbo].[Samples]", CommandType.Text).ConfigureAwait(false);

                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private IServiceCollection ConfigureServices()
        {
            var configruation = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configruation);

            //--Core
            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();
            serviceCollection.AddMemoryCache();

            //--Encryption
            serviceCollection.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            serviceCollection.AddSQLService<RunnerSQLServiceOptionsType, Configuration>();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();


            return serviceCollection;
        }
    }
}
