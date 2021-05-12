using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Models;
using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DickinsonBros.Core.Logger.Tests
{
    public class LoggingClass
    {
    }


    [TestClass]
    public class LoggerServiceTests : BaseTest
    {
        [TestMethod]
        public void LogDebugRedacted_Runs_LogCalledWithPramAsDebug()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var messageExpected = "LogMessage";
                    var keyExpected = "A";
                    var valueExpected = "B";
                    var correlationIdExpected = "123";
                    var CorrelationIdKeyExpected = "CorrelationId";
                    LogState stateObserved = null;
                    var properties = new Dictionary<string, object>
                    {
                        { keyExpected, valueExpected }
                    };

                    var propertiesRedactedExpected = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>(keyExpected, valueExpected),
                        new KeyValuePair<string, object>(CorrelationIdKeyExpected, correlationIdExpected)
                    };
                    var logStateExpected = new LogState(propertiesRedactedExpected);
                    var formatterResultExpected = "LogMessage\r\nA: B\r\nLogGroup: Application\r\nCorrelationId: 123\r\n";

                    var messageobserved = (string)null;
                    var propertiesRedactedObserved = (List<KeyValuePair<string, object>>)null;
                    var formatterResultObserved = (string)null;

                    var redactorServiceMock = serviceProvider.GetMock<IRedactorService>();
                    redactorServiceMock
                    .Setup
                    (
                        redactorService => redactorService.Redact
                        (
                            It.IsAny<object>()
                        )
                    )
                    .Returns(valueExpected);

                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                    .SetupGet(correlationService => correlationService.CorrelationId)
                    .Returns(correlationIdExpected);

                    var loggerMock = serviceProvider.GetMock<ILogger<LoggingClass>>();
                    loggerMock
                    .Setup
                    (
                        logger => logger.Log
                        (
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<object>(),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<object, Exception, string>>()
                        )
                    )
                    .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
                    {
                        stateObserved = (LogState)state;
                        messageobserved = (string)formatter.Target
                                                           .GetType()
                                                           .GetFields()
                                                           .First(e => e.Name == "message")
                                                           .GetValue(formatter.Target);

                        propertiesRedactedObserved = (List<KeyValuePair<string, object>>)formatter.Target
                                                                                                  .GetType()
                                                                                                  .GetFields()
                                                                                                  .First(e => e.Name == "propertiesRedacted")
                                                                                                  .GetValue(formatter.Target);

                        formatterResultObserved = formatter.Invoke(new object(), null);
                    });

                    var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                    var uutConcrete = (LoggerService<LoggingClass>)uut;

                    //Act
                    uutConcrete.LogDebugRedacted(messageExpected, Abstractions.Models.LogGroup.Application, properties);

                    //Assert
                    loggerMock
                    .Verify(
                        logger => logger.Log
                       (
                           LogLevel.Debug,
                           1,
                           It.IsAny<object>(),
                           null,
                           It.IsAny<Func<object, Exception, string>>()
                       ),
                        Times.Once
                    );

                    Assert.AreEqual(valueExpected, stateObserved._keyValuePairs.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, stateObserved._keyValuePairs.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(messageExpected, messageobserved);
                    Assert.AreEqual(valueExpected, propertiesRedactedObserved.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, propertiesRedactedObserved.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(formatterResultExpected, formatterResultObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void LogInformationRedacted_Runs_LogCalledWithPramAsInformation()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var messageExpected = "LogMessage";
                    var keyExpected = "A";
                    var valueExpected = "B";
                    var correlationIdExpected = "123";
                    var CorrelationIdKeyExpected = "CorrelationId";
                    LogState stateObserved = null;
                    var properties = new Dictionary<string, object>
                    {
                        { keyExpected, valueExpected }
                    };

                    var propertiesRedactedExpected = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>(keyExpected, valueExpected),
                        new KeyValuePair<string, object>(CorrelationIdKeyExpected, correlationIdExpected)
                    };
                    var logStateExpected = new LogState(propertiesRedactedExpected);
                    var formatterResultExpected = "LogMessage\r\nA: B\r\nLogGroup: Application\r\nCorrelationId: 123\r\n";

                    var messageobserved = (string)null;
                    var propertiesRedactedObserved = (List<KeyValuePair<string, object>>)null;
                    var formatterResultObserved = (string)null;

                    var redactorServiceMock = serviceProvider.GetMock<IRedactorService>();
                    redactorServiceMock
                    .Setup
                    (
                        redactorService => redactorService.Redact
                        (
                            It.IsAny<object>()
                        )
                    )
                    .Returns(valueExpected);

                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                    .SetupGet(correlationService => correlationService.CorrelationId)
                    .Returns(correlationIdExpected);

                    var loggerMock = serviceProvider.GetMock<ILogger<LoggingClass>>();
                    loggerMock
                    .Setup
                    (
                        logger => logger.Log
                        (
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<object>(),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<object, Exception, string>>()
                        )
                    )
                    .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
                    {
                        stateObserved = (LogState)state;
                        messageobserved = (string)formatter.Target
                                                           .GetType()
                                                           .GetFields()
                                                           .First(e => e.Name == "message")
                                                           .GetValue(formatter.Target);

                        propertiesRedactedObserved = (List<KeyValuePair<string, object>>)formatter.Target
                                                                                                  .GetType()
                                                                                                  .GetFields()
                                                                                                  .First(e => e.Name == "propertiesRedacted")
                                                                                                  .GetValue(formatter.Target);

                        formatterResultObserved = formatter.Invoke(new object(), null);
                    });

                    var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                    var uutConcrete = (LoggerService<LoggingClass>)uut;

                    //Act
                    uutConcrete.LogInformationRedacted(messageExpected, Abstractions.Models.LogGroup.Application, properties);

                    //Assert
                    loggerMock
                    .Verify(
                        logger => logger.Log
                       (
                           LogLevel.Information,
                           1,
                           It.IsAny<object>(),
                           null,
                           It.IsAny<Func<object, Exception, string>>()
                       ),
                        Times.Once
                    );

                    Assert.AreEqual(valueExpected, stateObserved._keyValuePairs.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, stateObserved._keyValuePairs.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(messageExpected, messageobserved);
                    Assert.AreEqual(valueExpected, propertiesRedactedObserved.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, propertiesRedactedObserved.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(formatterResultExpected, formatterResultObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void LogWarningRedacted_Runs_LogCalledWithPramAsWarning()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var messageExpected = "LogMessage";
                    var keyExpected = "A";
                    var valueExpected = "B";
                    var correlationIdExpected = "123";
                    var CorrelationIdKeyExpected = "CorrelationId";
                    LogState stateObserved = null;
                    var properties = new Dictionary<string, object>
                    {
                        { keyExpected, valueExpected }
                    };

                    var propertiesRedactedExpected = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>(keyExpected, valueExpected),
                        new KeyValuePair<string, object>(CorrelationIdKeyExpected, correlationIdExpected)
                    };
                    var logStateExpected = new LogState(propertiesRedactedExpected);
                    var formatterResultExpected = "LogMessage\r\nA: B\r\nLogGroup: Application\r\nCorrelationId: 123\r\n";

                    var messageobserved = (string)null;
                    var propertiesRedactedObserved = (List<KeyValuePair<string, object>>)null;
                    var formatterResultObserved = (string)null;

                    var redactorServiceMock = serviceProvider.GetMock<IRedactorService>();
                    redactorServiceMock
                    .Setup
                    (
                        redactorService => redactorService.Redact
                        (
                            It.IsAny<object>()
                        )
                    )
                    .Returns(valueExpected);

                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                    .SetupGet(correlationService => correlationService.CorrelationId)
                    .Returns(correlationIdExpected);

                    var loggerMock = serviceProvider.GetMock<ILogger<LoggingClass>>();
                    loggerMock
                    .Setup
                    (
                        logger => logger.Log
                        (
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<object>(),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<object, Exception, string>>()
                        )
                    )
                    .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
                    {
                        stateObserved = (LogState)state;
                        messageobserved = (string)formatter.Target
                                                           .GetType()
                                                           .GetFields()
                                                           .First(e => e.Name == "message")
                                                           .GetValue(formatter.Target);

                        propertiesRedactedObserved = (List<KeyValuePair<string, object>>)formatter.Target
                                                                                                  .GetType()
                                                                                                  .GetFields()
                                                                                                  .First(e => e.Name == "propertiesRedacted")
                                                                                                  .GetValue(formatter.Target);

                        formatterResultObserved = formatter.Invoke(new object(), null);
                    });

                    var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                    var uutConcrete = (LoggerService<LoggingClass>)uut;

                    //Act
                    uutConcrete.LogWarningRedacted(messageExpected, Abstractions.Models.LogGroup.Application, properties);

                    //Assert
                    loggerMock
                    .Verify(
                        logger => logger.Log
                       (
                           LogLevel.Warning,
                           1,
                           It.IsAny<object>(),
                           null,
                           It.IsAny<Func<object, Exception, string>>()
                       ),
                        Times.Once
                    );

                    Assert.AreEqual(valueExpected, stateObserved._keyValuePairs.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, stateObserved._keyValuePairs.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(messageExpected, messageobserved);
                    Assert.AreEqual(valueExpected, propertiesRedactedObserved.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, propertiesRedactedObserved.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(formatterResultExpected, formatterResultObserved);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void LogErrorRedacted_Runs_LogCalledWithPramAsError()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var messageExpected = "LogMessage";
                    var keyExpected = "A";
                    var valueExpected = "B";
                    var correlationIdExpected = "123";
                    var CorrelationIdKeyExpected = "CorrelationId";
                    var expectedException = new Exception();

                    LogState stateObserved = null;
                    var properties = new Dictionary<string, object>
                    {
                        { keyExpected, valueExpected }
                    };

                    var propertiesRedactedExpected = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>(keyExpected, valueExpected),
                        new KeyValuePair<string, object>(CorrelationIdKeyExpected, correlationIdExpected)
                    };
                    var logStateExpected = new LogState(propertiesRedactedExpected);
                    var formatterResultExpected = "LogMessage\r\nA: B\r\nLogGroup: Application\r\nCorrelationId: 123\r\n";

                    var messageobserved = (string)null;
                    var propertiesRedactedObserved = (List<KeyValuePair<string, object>>)null;
                    var formatterResultObserved = (string)null;

                    var redactorServiceMock = serviceProvider.GetMock<IRedactorService>();
                    redactorServiceMock
                    .Setup
                    (
                        redactorService => redactorService.Redact
                        (
                            It.IsAny<object>()
                        )
                    )
                    .Returns(valueExpected);

                    var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                    correlationServiceMock
                    .SetupGet(correlationService => correlationService.CorrelationId)
                    .Returns(correlationIdExpected);

                    var loggerMock = serviceProvider.GetMock<ILogger<LoggingClass>>();
                    loggerMock
                    .Setup
                    (
                        logger => logger.Log
                        (
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<object>(),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<object, Exception, string>>()
                        )
                    )
                    .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
                    {
                        stateObserved = (LogState)state;
                        messageobserved = (string)formatter.Target
                                                           .GetType()
                                                           .GetFields()
                                                           .First(e => e.Name == "message")
                                                           .GetValue(formatter.Target);

                        propertiesRedactedObserved = (List<KeyValuePair<string, object>>)formatter.Target
                                                                                                  .GetType()
                                                                                                  .GetFields()
                                                                                                  .First(e => e.Name == "propertiesRedacted")
                                                                                                  .GetValue(formatter.Target);

                        formatterResultObserved = formatter.Invoke(new object(), null);
                    });

                    var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                    var uutConcrete = (LoggerService<LoggingClass>)uut;

                    //Act
                    uutConcrete.LogErrorRedacted(messageExpected, Abstractions.Models.LogGroup.Application, expectedException, properties);

                    //Assert
                    loggerMock
                    .Verify(
                        logger => logger.Log
                       (
                           LogLevel.Error,
                           1,
                           It.IsAny<object>(),
                           expectedException,
                           It.IsAny<Func<object, Exception, string>>()
                       ),
                        Times.Once
                    );

                    Assert.AreEqual(valueExpected, stateObserved._keyValuePairs.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, stateObserved._keyValuePairs.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(messageExpected, messageobserved);
                    Assert.AreEqual(valueExpected, propertiesRedactedObserved.First(e => e.Key == keyExpected).Value);
                    Assert.AreEqual(correlationIdExpected, propertiesRedactedObserved.First(e => e.Key == CorrelationIdKeyExpected).Value);
                    Assert.AreEqual(formatterResultExpected, formatterResultObserved);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Log_Runs_LogCalledWithPrams()
        {
            RunDependencyInjectedTest
          (
              (serviceProvider) =>
              {
                  //Setup
                  var messageExpected = "LogMessage";
                  var keyExpected = "A";
                  var valueExpected = "B";
                  var correlationIdExpected = "123";
                  var CorrelationIdKeyExpected = "CorrelationId";
                  LogState stateObserved = null;
                  var properties = new Dictionary<string, object>
                  {
                        { keyExpected, valueExpected }
                  };

                  var propertiesRedactedExpected = new List<KeyValuePair<string, object>>()
                  {
                        new KeyValuePair<string, object>(keyExpected, valueExpected),
                        new KeyValuePair<string, object>(CorrelationIdKeyExpected, correlationIdExpected)
                  };
                  var logStateExpected = new LogState(propertiesRedactedExpected);
                  var formatterResultExpected = "LogMessage\r\nA: B\r\nLogGroup: Application\r\nCorrelationId: 123\r\n";
                  var logLevelExpected = LogLevel.Information;

                  var messageobserved = (string)null;
                  var propertiesRedactedObserved = (List<KeyValuePair<string, object>>)null;
                  var formatterResultObserved = (string)null;

                  var redactorServiceMock = serviceProvider.GetMock<IRedactorService>();
                  redactorServiceMock
                  .Setup
                  (
                      redactorService => redactorService.Redact
                      (
                          It.IsAny<object>()
                      )
                  )
                  .Returns(valueExpected);

                  var correlationServiceMock = serviceProvider.GetMock<ICorrelationService>();
                  correlationServiceMock
                  .SetupGet(correlationService => correlationService.CorrelationId)
                  .Returns(correlationIdExpected);

                  var loggerMock = serviceProvider.GetMock<ILogger<LoggingClass>>();
                  loggerMock
                  .Setup
                  (
                      logger => logger.Log
                      (
                          It.IsAny<LogLevel>(),
                          It.IsAny<EventId>(),
                          It.IsAny<object>(),
                          It.IsAny<Exception>(),
                          It.IsAny<Func<object, Exception, string>>()
                      )
                  )
                  .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
                  {
                      stateObserved = (LogState)state;
                      messageobserved = (string)formatter.Target
                                                         .GetType()
                                                         .GetFields()
                                                         .First(e => e.Name == "message")
                                                         .GetValue(formatter.Target);

                      propertiesRedactedObserved = (List<KeyValuePair<string, object>>)formatter.Target
                                                                                                .GetType()
                                                                                                .GetFields()
                                                                                                .First(e => e.Name == "propertiesRedacted")
                                                                                                .GetValue(formatter.Target);

                      formatterResultObserved = formatter.Invoke(new object(), null);
                  });

                  var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                  var uutConcrete = (LoggerService<LoggingClass>)uut;

                  //Act
                  uutConcrete.Log(logLevelExpected, messageExpected, Abstractions.Models.LogGroup.Application, properties);

                  //Assert
                  loggerMock
                .Verify(
                    logger => logger.Log
                   (
                       logLevelExpected,
                       1,
                       It.IsAny<object>(),
                       null,
                       It.IsAny<Func<object, Exception, string>>()
                   ),
                    Times.Once
                );

                  Assert.AreEqual(valueExpected, stateObserved._keyValuePairs.First(e => e.Key == keyExpected).Value);
                  Assert.AreEqual(correlationIdExpected, stateObserved._keyValuePairs.First(e => e.Key == CorrelationIdKeyExpected).Value);
                  Assert.AreEqual(messageExpected, messageobserved);
                  Assert.AreEqual(valueExpected, propertiesRedactedObserved.First(e => e.Key == keyExpected).Value);
                  Assert.AreEqual(correlationIdExpected, propertiesRedactedObserved.First(e => e.Key == CorrelationIdKeyExpected).Value);
                  Assert.AreEqual(formatterResultExpected, formatterResultObserved);

              },
              serviceCollection => ConfigureServices(serviceCollection)
          );
        }

        [TestMethod]
        public void Formatter_Runs_ReturnsFormattedMessage()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                  //Setup
                  var message = "LogMessage";
                    var propertiesRedacted = new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("A", "B"),
                        new KeyValuePair<string, object>("CorrelationId", "123")
                    };

                    var formatterResultExpected = "LogMessage\r\nA: B\r\nCorrelationId: 123\r\n";

                    var uut = serviceProvider.GetRequiredService<ILoggerService<LoggingClass>>();
                    var uutConcrete = (LoggerService<LoggingClass>)uut;

                  //Act
                  var observed = uutConcrete.Formatter(message, propertiesRedacted);

                  //Assert
                  Assert.AreEqual(formatterResultExpected, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILoggerService<LoggingClass>, LoggerService<LoggingClass>>();
            serviceCollection.AddSingleton(Mock.Of<IRedactorService>());
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<ILogger<LoggingClass>>());
            return serviceCollection;
        }

    }
}
