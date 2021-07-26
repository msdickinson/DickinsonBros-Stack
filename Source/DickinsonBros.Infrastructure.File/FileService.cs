using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.File.Abstractions;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.File
{
    public class FileService : IFileService
    {
        internal readonly ICorrelationService _correlationService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ILoggerService<FileService> _logger;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly IFileSystem _fileSystem;

        public FileService
        (
            ICorrelationService correlationService,
            IDateTimeService dateTimeService,
            ILoggerService<FileService> logger,
            IStopwatchFactory stopwatchFactory,
            ITelemetryWriterService telemetryWriterService,
            IFileSystem fileSystem
        )
        {
            _correlationService = correlationService;
            _dateTimeService = dateTimeService;
            _logger = logger;
            _stopwatchFactory = stopwatchFactory;
            _telemetryWriterService = telemetryWriterService;
            _fileSystem = fileSystem;
        }
   
        public void DeleteFile(string path)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.DeleteFile)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"CreateFile",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId,
                TelemetryResponseState = TelemetryResponseState.Successful
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();


            try
            {
                stopwatchService.Start();
                _fileSystem.File.Delete(path);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }

        public bool FileExists(string path)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.FileExists)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"FileExists",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                stopwatchService.Start();
                var response = _fileSystem.File.Exists(path);
                stopwatchService.Stop();

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }

        }

        public async Task<string> LoadFileAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"LoadFile - String",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                stopwatchService.Start();
                var response = await _fileSystem.File.ReadAllTextAsync(path, encoding, cancellationToken).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }

        public async Task<byte[]> LoadFileAsync(string path, CancellationToken cancellationToken = default)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"LoadFile - Byte Array",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                stopwatchService.Start();
                var response = await _fileSystem.File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }

        public async Task UpsertFileAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.UpsertFileAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"UpsertFile",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                stopwatchService.Start();
                await _fileSystem.File.WriteAllBytesAsync(path, bytes, cancellationToken).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }

        public async Task UpsertFileAsync(string path, string file, Encoding encoding, CancellationToken cancellationToken = default)
        {
            var methodIdentifier = $"{nameof(IFileService)}.{nameof(IFileService.UpsertFileAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = "File System",
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"UpsertFileAsync",
                TelemetryType = TelemetryType.FileSystem,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();

            try
            {
                stopwatchService.Start();
                await _fileSystem.File.WriteAllTextAsync(path, file, encoding, cancellationToken).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(path), path },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
            }
            finally
            {
                _telemetryWriterService.Insert(insertTelemetryRequest);
            }
        }
    }
}
