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
using DickinsonBros.Infrastructure.File.Abstractions;
using DickinsonBros.Infrastructure.File.AspDI.Extensions;
using DickinsonBros.Infrastructure.File.Runner.AspDI.Config;
using DickinsonBros.Sinks.Telemetry.AzureTables.Abstractions;
using DickinsonBros.Sinks.Telemetry.AzureTables.AspDI.Extensions;
using DickinsonBros.Sinks.Telemetry.Log.Abstractions;
using DickinsonBros.Sinks.Telemetry.Log.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.File.Runner.AspDI
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
                var fileService = provider.GetRequiredService<IFileService>();
                var sinksTelemetryLogService = provider.GetRequiredService<ISinksTelemetryLogService>();
                var sinksTelemetryAzureTablesService = provider.GetRequiredService<ISinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>>();

                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var filePath = "./";
                var filename = Guid.NewGuid().ToString() + ".txt";
                var fileContent = "Sample File";

                //Upsert (Create)
                await fileService.UpsertFileAsync(filePath + filename, fileContent, Encoding.ASCII).ConfigureAwait(false);

                //Upsert (Replace)
                await fileService.UpsertFileAsync(filePath + filename, fileContent + " Edited", Encoding.ASCII).ConfigureAwait(false);

                //Load (Byte[])
                var exist = fileService.FileExists(filePath + filename);

                //Load (String)
                var text = await fileService.LoadFileAsync(filePath + filename, Encoding.ASCII).ConfigureAwait(false);

                //Load (Byte[])
                var byteArray = await fileService.LoadFileAsync(filePath + filename).ConfigureAwait(false);

                //Delete
                fileService.DeleteFile(filePath + filename);

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

            //--Misc
            serviceCollection.TryAddSingleton<IFileSystem, FileSystem>();

            //--Core
            serviceCollection.AddDateTimeService();
            serviceCollection.AddGuidService();
            serviceCollection.AddStopwatchService();
            serviceCollection.AddStopwatchFactory();
            serviceCollection.AddCorrelationService();
            serviceCollection.AddRedactorService();
            serviceCollection.AddLoggerService();
            serviceCollection.AddTelemetryWriterService();

            //--Encryption
            serviceCollection.AddCertificateEncryptionService<Configuration>();

            //--Infrastructure
            serviceCollection.AddFileService();
            serviceCollection.AddAzureTablesService<RunnerAzureTableServiceOptionsType, Configuration>();

            //--Sinks
            serviceCollection.AddSinksTelemetryAzureTablesService<RunnerAzureTableServiceOptionsType>();
            serviceCollection.AddSinksTelemetryLogServiceService();


            return serviceCollection;
        }
    }
}
