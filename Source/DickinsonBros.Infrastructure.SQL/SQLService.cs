using Dapper;
using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL
{
    [ExcludeFromCodeCoverage]
    public class SQLService<U> : ISQLService<U>
    where U : SQLServiceOptionsType
    {
        internal readonly ILoggerService<SQLService<U>> _logger;
        internal readonly TimeSpan DefaultBulkCopyTimeout = TimeSpan.FromMinutes(5);
        internal readonly int DefaultBatchSize = 10000;
        internal readonly ITelemetryWriterService _telemetryWriterService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly IRedactorService _redactorService;
        internal readonly ICorrelationService _correlationService;
        internal readonly SQLServiceOptions<U> _sqlServiceOptions;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly IDataTableService _dataTableService;

        public SQLService
        (
            ILoggerService<SQLService<U>> logger,
            IRedactorService redactorService,
            ITelemetryWriterService telemetryWriterService,
            IDateTimeService dateTimeService,
            IStopwatchFactory stopwatchFactory,
            ICorrelationService correlationService,
            IOptions<SQLServiceOptions<U>> options,
            IDataTableService dataTableService
        )
        {
            _logger = logger;
            _telemetryWriterService = telemetryWriterService;
            _redactorService = redactorService;
            _correlationService = correlationService;
            _dateTimeService = dateTimeService;
            _sqlServiceOptions = options.Value;
            _stopwatchFactory = stopwatchFactory;
            _dataTableService = dataTableService;
        }

        public async Task BulkCopyAsync(DataTable dataTable, string tableName, int? batchSize, TimeSpan? timeout, CancellationToken? token)
        {
            var methodIdentifier = $"{nameof(SQLService<U>)}<{typeof(U).Name}>.{nameof(SQLService<U>.ExecuteAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _sqlServiceOptions.ConnectionName,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Bulk Copy TableName ({tableName})",
                TelemetryType = TelemetryType.SQL,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                if (dataTable == null)
                {
                    throw new ArgumentNullException(nameof(dataTable));
                }
                if (dataTable.Rows.Count == 0)
                {
                    return;
                }

                stopwatchService.Start();
                using SqlConnection connection = new SqlConnection(_sqlServiceOptions.ConnectionString);
                await connection.OpenAsync(token ?? CancellationToken.None).ConfigureAwait(false);

                using SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, null)
                {
                    DestinationTableName = tableName,
                    BulkCopyTimeout = (int)(timeout ?? DefaultBulkCopyTimeout).TotalSeconds,
                    BatchSize = batchSize ?? DefaultBatchSize
                };
                for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                {
                    DataColumn dataColumn = dataTable.Columns[columnIndex];
                    bulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName);
                }
                await bulkCopy.WriteToServerAsync(dataTable, token ?? CancellationToken.None).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(dataTable), dataTable },
                        { nameof(tableName), tableName },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(dataTable), dataTable },
                        { nameof(tableName), tableName },
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

        public async Task BulkCopyAsync<T>(IEnumerable<T> enumerable, string tableName, int? batchSize, TimeSpan? timeout, CancellationToken? token)
        {
            var dataTable = _dataTableService.ToDataTable(enumerable, tableName);
            await BulkCopyAsync(dataTable, tableName, batchSize, timeout, token).ConfigureAwait(false);
        }

        public async Task ExecuteAsync(string sql, CommandType commandType, object param = null)
        {
            var methodIdentifier = $"{nameof(SQLService<U>)}<{typeof(U).Name}>.{nameof(SQLService<U>.ExecuteAsync)}";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _sqlServiceOptions.ConnectionName,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Execute SQL - {sql}",
                TelemetryType = TelemetryType.SQL,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                stopwatchService.Start();
                using SqlConnection connection = new SqlConnection(_sqlServiceOptions.ConnectionString);
                await connection.OpenAsync().ConfigureAwait(false);
                await connection.ExecuteAsync(
                    sql,
                    param,
                    commandType: commandType).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
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

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, CommandType commandType, object param = null)
        {
            var methodIdentifier = $"{nameof(SQLService<U>)}<{typeof(U).Name}>.{nameof(SQLService<U>.QueryAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _sqlServiceOptions.ConnectionName,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Query {typeof(T).Name} SQL - {sql}",
                TelemetryType = TelemetryType.SQL,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                stopwatchService.Start();
                using SqlConnection connection = new SqlConnection(_sqlServiceOptions.ConnectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                var response = await connection.QueryAsync<T>(
                  sql,
                  param,
                  commandType: commandType).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(response), response },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
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

        public async Task<T> QueryFirstAsync<T>(string sql, CommandType commandType, object param = null)
        {
            var methodIdentifier = $"{nameof(SQLService<U>)}<{typeof(U).Name}>.{nameof(SQLService<U>.QueryAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _sqlServiceOptions.ConnectionName,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"QueryFirst {typeof(T).Name} SQL - {sql}",
                TelemetryType = TelemetryType.SQL,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                stopwatchService.Start();
                using SqlConnection connection = new SqlConnection(_sqlServiceOptions.ConnectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                var response = await connection.QueryFirstAsync<T>(
                  sql,
                  param,
                  commandType: commandType).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(response), response },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
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

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, CommandType commandType, object param = null)
        {
            var methodIdentifier = $"{nameof(SQLService<U>)}<{typeof(U).Name}>.{nameof(SQLService<U>.QueryFirstOrDefaultAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _sqlServiceOptions.ConnectionName,
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"QueryFirstOrDefault {typeof(T).Name} SQL - {sql}",
                TelemetryType = TelemetryType.SQL,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            try
            {
                stopwatchService.Start();
                using SqlConnection connection = new SqlConnection(_sqlServiceOptions.ConnectionString);
                await connection.OpenAsync().ConfigureAwait(false);

                var response = await connection.QueryFirstOrDefaultAsync<T>(
                  sql,
                  param,
                  commandType: commandType).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(response), response },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (Exception exception)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.UnhandledException;
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;

                _logger.LogErrorRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    exception,
                    new Dictionary<string, object>
                    {
                        { nameof(sql), sql },
                        { nameof(param), param },
                        { nameof(commandType), Enum.GetName(typeof(CommandType), commandType) },
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
