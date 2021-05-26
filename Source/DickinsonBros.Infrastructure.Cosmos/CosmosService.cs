using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.Cosmos.Abstractions;
using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.Cosmos
{
    public class CosmosService<U> : ICosmosService<U>
    where U : CosmosServiceOptionsType
    {
        internal readonly Container _cosmosContainer;
        internal readonly CosmosClient _cosmosClient;
        internal readonly ICorrelationService _correlationService;
        internal readonly IDateTimeService _dateTimeService;
        internal readonly ILoggerService<ICosmosService<U>> _logger;
        internal readonly IStopwatchFactory _stopwatchFactory;
        internal readonly ITelemetryWriterService _telemetryWriterService;

        public CosmosService
        (
           
            ICorrelationService correlationService,
            ICosmosFactory cosmosFactory, 
            IDateTimeService dateTimeService,
            IOptions<CosmosServiceOptions<U>> options,
            IStopwatchFactory stopwatchFactory,
            ITelemetryWriterService telemetryWriterService,
            ILoggerService<CosmosService<U>> logger
        )
        {
            _cosmosClient = cosmosFactory.CreateCosmosClient(options.Value);
            _cosmosContainer = cosmosFactory.GetContainer(_cosmosClient, options.Value);
            _correlationService = correlationService;
            _dateTimeService = dateTimeService;
            _telemetryWriterService = telemetryWriterService;
            _stopwatchFactory = stopwatchFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.QueryAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Query {typeof(T).Name}. QueryDefinition: {queryDefinition.QueryText}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();
            try
            {
                var items = new List<T>();
                using (FeedIterator<T> feedIterator = _cosmosContainer.GetItemQueryIterator<T>(queryDefinition, null, queryRequestOptions))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        var response = await feedIterator.ReadNextAsync();
                        foreach (var item in response)
                        {
                            items.Add(item);
                        }
                    }
                };
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(queryDefinition), queryDefinition },
                        { nameof(queryRequestOptions), queryRequestOptions },
                        { nameof(items), items },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return items;
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
                        { nameof(queryDefinition), queryDefinition },
                        { nameof(queryRequestOptions), queryRequestOptions },
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

        public async Task<ItemResponse<T>> FetchAsync<T>(string id, string key) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.FetchAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Fetch {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var response = await _cosmosContainer.ReadItemAsync<T>(id, new PartitionKey(key)).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(id), id },
                        { nameof(key), key },
                        { nameof(response), response },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.NotFound;

                _logger.LogInformationRedacted
                (
                    $"{methodIdentifier} NotFound",
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(id), id },
                        { nameof(key), key },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
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
                        { nameof(id), id },
                        { nameof(key), key },
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
      
        public async Task<ItemResponse<T>> InsertAsync<T>(T value) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.InsertAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Insert {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var response = await _cosmosContainer.CreateItemAsync<T>(value, new PartitionKey(value.Key)).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(value.Key), value.Key },
                        { nameof(value), value },
                        { nameof(response), response },
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
                        { nameof(value), value },
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

        public async Task<IEnumerable<ResponseMessage>> InsertBulkAsync<T>(IEnumerable<T> items) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.InsertBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"InsertBulk {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var responseMessages = new List<ResponseMessage>();

                Dictionary<PartitionKey, Stream> itemsToInsert = new Dictionary<PartitionKey, Stream>();
                List<Task> tasks = new List<Task>();
                foreach (T item in items)
                {
                    MemoryStream stream = new MemoryStream();
                    await JsonSerializer.SerializeAsync(stream, item, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ConfigureAwait(false);
                    var partitionKey = new PartitionKey(item.Key);
                    tasks.Add(_cosmosContainer.CreateItemStreamAsync(stream, new PartitionKey(item.Key))
                      .ContinueWith((Task<ResponseMessage> task) =>
                      {
                          using (ResponseMessage response = task.Result)
                          {
                              responseMessages.Add(response);
                              if (!response.IsSuccessStatusCode)
                              {
                                  _logger.LogErrorRedacted
                                  (
                                      $"{methodIdentifier} Item Response",
                                      LogGroup.Infrastructure,
                                      null,
                                      new Dictionary<string, object>
                                      {
                                            { nameof(item), item },
                                            { nameof(response.StatusCode), response.StatusCode },
                                            { nameof(response.ErrorMessage), response.ErrorMessage }
                                      }
                                  );
                              }
                          }
                      }));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(items), items },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return responseMessages;
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
                        { nameof(items), items },
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

        public async Task<ItemResponse<T>> UpsertAsync<T>(T value) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.UpsertAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Upsert {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var response = await _cosmosContainer.UpsertItemAsync<T>(value, new PartitionKey(value.Key), new ItemRequestOptions { IfMatchEtag = value._etag }).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(value), value },
                        { nameof(response), response },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return response;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Conflict;
                _logger.LogInformationRedacted
                (
                    $"{methodIdentifier} PreconditionFailed",
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(value), value },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                throw;
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
                        { nameof(value), value },
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

        public async Task<IEnumerable<ResponseMessage>> UpsertBulkAsync<T>(IEnumerable<T> items) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.UpsertBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"UpsertBulk {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var responseMessages = new List<ResponseMessage>();
                Dictionary<PartitionKey, Stream> itemsToInsert = new Dictionary<PartitionKey, Stream>();
                List<Task> tasks = new List<Task>();
                foreach (T item in items)
                {
                    MemoryStream stream = new MemoryStream();
                    await JsonSerializer.SerializeAsync(stream, item, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ConfigureAwait(false);
                    tasks.Add(_cosmosContainer.UpsertItemStreamAsync(stream, new PartitionKey(item.Key), new ItemRequestOptions { IfMatchEtag = item._etag })
                      .ContinueWith((Task<ResponseMessage> task) =>
                      {
                          using (ResponseMessage response = task.Result)
                          {
                              responseMessages.Add(response);
                              if (!response.IsSuccessStatusCode)
                              {
                                  _logger.LogErrorRedacted
                                  (
                                      $"{methodIdentifier} Item Response",
                                      LogGroup.Infrastructure,
                                      null,
                                      new Dictionary<string, object>
                                      {
                                            { nameof(item), item },
                                            { nameof(response.StatusCode), response.StatusCode },
                                            { nameof(response.ErrorMessage), response.ErrorMessage }
                                      }
                                  );
                              }
                          }
                      }));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);

                stopwatchService.Stop();
                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(items), items },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return responseMessages;
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
                        { nameof(items), items },
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

        public async Task<ItemResponse<T>> DeleteAsync<T>(string id, string key) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.DeleteAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"Delete {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var response = await _cosmosContainer.DeleteItemAsync<T>(id, new PartitionKey(key)).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(id), id },
                        { nameof(key), key },
                        { nameof(response), response },
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
                        { nameof(id), id },
                        { nameof(key), key },
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

        public async Task<IEnumerable<ResponseMessage>> DeleteBulkAsync<T>(IEnumerable<T> items) where T : CosmosEntity
        {
            var methodIdentifier = $"{nameof(ICosmosService<U>)}<{typeof(U).Name}>.{nameof(ICosmosService<U>.DeleteBulkAsync)}<{typeof(T).Name}>";

            var insertTelemetryRequest = new InsertTelemetryItem
            {
                ConnectionName = _cosmosClient.Endpoint.ToString(),
                DateTimeUTC = _dateTimeService.GetDateTimeUTC(),
                Request = $"DeleteBulk {typeof(T).Name}",
                TelemetryType = TelemetryType.Cosmos,
                CorrelationId = _correlationService.CorrelationId
            };

            var stopwatchService = _stopwatchFactory.NewStopwatchService();
            stopwatchService.Start();

            try
            {
                var responseMessages = new List<ResponseMessage>();

                Dictionary<PartitionKey, Stream> itemsToInsert = new Dictionary<PartitionKey, Stream>();
                List<Task> tasks = new List<Task>();
                foreach (var item in items)
                {
                    tasks.Add(_cosmosContainer.DeleteItemStreamAsync(item.Id, new PartitionKey(item.Key))
                      .ContinueWith((Task<ResponseMessage> task) =>
                      {
                          using (ResponseMessage response = task.Result)
                          {
                              responseMessages.Add(response);
                              if (!response.IsSuccessStatusCode)
                              {
                                  _logger.LogErrorRedacted
                                  (
                                      $"{methodIdentifier} Item Response",
                                      LogGroup.Infrastructure,
                                      null,
                                      new Dictionary<string, object>
                                      {
                                            { nameof(item), item },
                                            { nameof(response.StatusCode), response.StatusCode },
                                            { nameof(response.ErrorMessage), response.ErrorMessage }
                                      }
                                  );
                              }
                          }
                      }));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                stopwatchService.Stop();

                insertTelemetryRequest.Duration = stopwatchService.Elapsed;
                insertTelemetryRequest.TelemetryResponseState = TelemetryResponseState.Successful;

                _logger.LogInformationRedacted
                (
                    methodIdentifier,
                    LogGroup.Infrastructure,
                    new Dictionary<string, object>
                    {
                        { nameof(items), items },
                        { nameof(insertTelemetryRequest.Duration), insertTelemetryRequest.Duration },
                        { nameof(insertTelemetryRequest.TelemetryResponseState), insertTelemetryRequest.TelemetryResponseState }
                    }
                );

                return responseMessages;
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
                        { nameof(items), items },
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
