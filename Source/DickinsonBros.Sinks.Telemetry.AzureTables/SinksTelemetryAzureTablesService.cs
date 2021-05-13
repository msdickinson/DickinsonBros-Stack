using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.AzureTables.Abstractions;
using DickinsonBros.Infrastructure.AzureTables.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Sinks.Telemetry.AzureTables
{
    public class SinksTelemetryAzureTablesService<U> : ITelemetryServiceWriter
    where U : AzureTableServiceOptionsType
    {
        // TODO:
        internal readonly IAzureTableService<U> _azureTableService;

        //Find better thread safe libary like concurete dictionary or somthing
        internal readonly IList<InsertTelemetryRequest> items;

        //Add a on x amount of time QUE results
        //Determine if on close a proper way to ensure it waiting exists. Other wise mannaul.

        public SinksTelemetryAzureTablesService
        (
            IAzureTableService<U> azureTableService
        )
        {
            _azureTableService = azureTableService;
        }

        public async Task InsertAsync(InsertTelemetryRequest telemetryItem)
        {
            // TODO:
            items.Add(telemetryItem);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            // TODO:
            //Pull From pooled items
            //Convert each item to a ITableEntity class type
            //Do bulk insert.

           // await _azureTableService.InsertBulkAsync(items, "", false).ConfigureAwait(false);

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
