using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Infrastructure.File.Abstractions;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.File.Tests
{
    [TestClass]
    public class FileServiceTests : BaseTest
    {
        #region DeleteFile

        [TestMethod]
        public async Task DeleteFile_Runs_GetDateTimeUTCCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;
                  
                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_IFileSystemDeleteCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    fileSystemMock
                    .Verify
                    (
                        fileSystem => fileSystem.File.Delete(path),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.DeleteFile(path);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.DeleteFile)}",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    Assert.ThrowsException<Exception>(() => uutConcrete.DeleteFile(path));

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.DeleteFile)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteFile_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    Assert.ThrowsException<Exception>(() => uutConcrete.DeleteFile(path));

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("File System", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.FileSystem, insertTelemetryRequestObserved.TelemetryType);

                   
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }


        #endregion

        #region FileExists

        [TestMethod]
        public async Task FileExists_Runs_GetDateTimeUTCCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_IFileSystemFileExistsCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    fileSystemMock
                    .Verify
                    (
                        fileSystem => fileSystem.File.Exists(path),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    uutConcrete.FileExists(path);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.FileExists)}",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    Assert.ThrowsException<Exception>(() => uutConcrete.FileExists(path));

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.FileExists)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FileExists_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    Assert.ThrowsException<Exception>(() => uutConcrete.FileExists(path));

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("File System", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.FileSystem, insertTelemetryRequestObserved.TelemetryType);


                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }


        #endregion

        #region LoadFileAsync

        [TestMethod]
        public async Task LoadFileAsync_Runs_GetDateTimeUTCCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_IFileSystemLoadFileAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    fileSystemMock
                    .Verify
                    (
                        fileSystem => fileSystem.File.ReadAllBytesAsync(path, cancellationToken),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, cancellationToken).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.LoadFileAsync(path).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsync_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.LoadFileAsync(path).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("File System", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.FileSystem, insertTelemetryRequestObserved.TelemetryType);


                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region LoadFileAsyncString

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_GetDateTimeUTCCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    dateTimeServiceMock
                    .Verify
                    (
                        dateTimeService => dateTimeService.GetDateTimeUTC(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_NewStopwatchServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchFactoryMock
                    .Verify
                    (
                        stopwatchFactory => stopwatchFactory.NewStopwatchService(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_StopwatchServiceStartCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Start(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_IFileSystemLoadFileAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    fileSystemMock
                    .Verify
                    (
                        fileSystem => fileSystem.File.ReadAllTextAsync(path, encoding, cancellationToken),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_StopwatchServiceStopCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    stopwatchServiceMock
                    .Verify
                    (
                        stopwatchService => stopwatchService.Stop(),
                        Times.Once()
                    );

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_LogInformationRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogInformationRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.Successful, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Throws_LogErrorRedactedCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath"; 
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    loggerServiceMock
                    .Verify
                    (
                        loggerService => loggerService.LogErrorRedacted
                        (
                            $"{nameof(IFileService)}.{nameof(IFileService.LoadFileAsync)}",
                            LogGroup.Infrastructure,
                            It.IsAny<Exception>(),
                            It.IsAny<IDictionary<string, object>>()
                        ),
                        Times.Once
                    );

                    var pathObserved = (string)propertiesObserved["path"];
                    var durationObserved = (TimeSpan)propertiesObserved["Duration"];
                    var telemetryResponseStateObserved = (TelemetryResponseState)propertiesObserved["TelemetryResponseState"];

                    Assert.AreEqual(path, pathObserved);
                    Assert.AreEqual(0, durationObserved.TotalMilliseconds);
                    Assert.AreEqual(TelemetryResponseState.UnhandledException, telemetryResponseStateObserved);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LoadFileAsyncString_Runs_TelemetryWriterServiceCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup

                    //-- Inputs
                    var path = "SamplePath";
                    var encoding = Encoding.UTF8;
                    var cancellationToken = new CancellationToken();

                    //-- IDateTimeService
                    var dateTime = new DateTime(2000, 1, 1);
                    var dateTimeServiceMock = CreateDateTimeServiceMock(serviceProvider, dateTime);

                    //-- IStopwatchService
                    var stopwatchServiceMock = CreateStopWatchServiceMock(serviceProvider);

                    //-- IStopwatchFactory
                    var stopwatchFactoryMock = CreateStopWatchFactoryMock(serviceProvider, stopwatchServiceMock.Object);

                    //-- IFile
                    var fileMock = CreateFileMock(serviceProvider);

                    //-- IFileSystem
                    var fileSystemMock = CreateFileSystemMock(serviceProvider, fileMock.Object, true);

                    //-- ILoggerService
                    var (loggerServiceMock, propertiesObserved) = CreateLoggerService(serviceProvider);

                    //-- ITelemetryServiceWriter
                    var (telemetryServiceWriterMock, insertTelemetryRequestObserved) = CreateTelemetryWriterServiceMock(serviceProvider);

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IFileService>();
                    var uutConcrete = (FileService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<Exception>(async () => await uutConcrete.LoadFileAsync(path, encoding, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                    telemetryServiceWriterMock
                    .Verify
                    (
                        telemetryServiceWriter => telemetryServiceWriter.Insert
                        (
                            It.IsAny<InsertTelemetryItem>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual("File System", insertTelemetryRequestObserved.ConnectionName);
                    Assert.AreEqual(TelemetryType.FileSystem, insertTelemetryRequestObserved.TelemetryType);


                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            ).ConfigureAwait(false);
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            //--Misc
            serviceCollection.AddSingleton(Mock.Of<IFile>());
            serviceCollection.AddSingleton(Mock.Of<IFileSystem>());

            //--Core
            serviceCollection.AddSingleton(Mock.Of<ICorrelationService>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchFactory>());
            serviceCollection.AddSingleton(Mock.Of<IStopwatchService>());
            serviceCollection.AddSingleton(Mock.Of<IDateTimeService>());
            serviceCollection.AddSingleton(Mock.Of<ILoggerService<FileService>>());
            serviceCollection.AddSingleton(Mock.Of<ITelemetryWriterService>());

            //--Infrastructure
            serviceCollection.AddSingleton<IFileService, FileService>();

            return serviceCollection;
        }

        private Mock<IDateTimeService> CreateDateTimeServiceMock(IServiceProvider serviceProvider, DateTime getDateTimeUTCResponse)
        {
            var dateTimeServiceMock = serviceProvider.GetMock<IDateTimeService>();
            dateTimeServiceMock.Setup(stopwatchService => stopwatchService.GetDateTimeUTC()).Returns(getDateTimeUTCResponse);

            return dateTimeServiceMock;
        }

        private Mock<IStopwatchService> CreateStopWatchServiceMock(IServiceProvider serviceProvider)
        {
            var stopwatchServiceMock = serviceProvider.GetMock<IStopwatchService>();
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Start());
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.ElapsedMilliseconds).Returns(0);
            stopwatchServiceMock.Setup(stopwatchService => stopwatchService.Stop());

            return stopwatchServiceMock;
        }

        private Mock<IFile> CreateFileMock(IServiceProvider serviceProvider)
        {
            var fileMock = serviceProvider.GetMock<IFile>();

            return fileMock;
        }
        private Mock<IFileSystem> CreateFileSystemMock(IServiceProvider serviceProvider, IFile file, bool shouldthrow = false)
        {
            var fileSystemMock = serviceProvider.GetMock<IFileSystem>();

            if(shouldthrow)
            {
                fileSystemMock
                .Setup(fileSystem => fileSystem.File)
                .Throws(new Exception());

                return fileSystemMock; 
            }

            fileSystemMock
            .Setup(fileSystem => fileSystem.File)
            .Returns(file);

            return fileSystemMock;
        }
        

        private Mock<IStopwatchFactory> CreateStopWatchFactoryMock(IServiceProvider serviceProvider, IStopwatchService stopwatchService)
        {
            var stopwatchFactoryMock = serviceProvider.GetMock<IStopwatchFactory>();

            stopwatchFactoryMock
            .Setup(stopwatchFactory => stopwatchFactory.NewStopwatchService())
            .Returns(stopwatchService);

            return stopwatchFactoryMock;
        }


        private (Mock<ILoggerService<FileService>>,Dictionary<string, object>) CreateLoggerService(IServiceProvider serviceProvider)
        {
            var loggerServiceMock = serviceProvider.GetMock<ILoggerService<FileService>>();
            var propertiesObserved = new Dictionary<string, object>();
            loggerServiceMock
            .Setup
            (
                loggerService => loggerService.LogInformationRedacted
                (
                    It.IsAny<string>(),
                    It.IsAny<LogGroup>(),
                    It.IsAny<IDictionary<string, object>>()
                )
            ).Callback((string message, LogGroup LogGroup, IDictionary<string, object> properties) =>
            {
                foreach (var prop in properties)
                {
                    propertiesObserved.Add(prop.Key, prop.Value);
                }
            });

            loggerServiceMock
            .Setup
            (
                loggerService => loggerService.LogErrorRedacted
                (
                    It.IsAny<string>(),
                    It.IsAny<LogGroup>(),
                    It.IsAny<Exception>(),
                    It.IsAny<IDictionary<string, object>>()
                )
            ).Callback((string message, LogGroup LogGroup, Exception exception, IDictionary<string, object> properties) =>
            {
                foreach (var prop in properties)
                {
                    propertiesObserved.Add(prop.Key, prop.Value);
                }
            });

            return (loggerServiceMock, propertiesObserved);
        }
        private (Mock<ITelemetryWriterService>, InsertTelemetryItem) CreateTelemetryWriterServiceMock(IServiceProvider serviceProvider)
        {
            var insertTelemetryRequestObserved = new InsertTelemetryItem();
            var telemetryServiceWriterMock = serviceProvider.GetMock<ITelemetryWriterService>();
            telemetryServiceWriterMock
            .Setup
            (
                telemetryServiceWriter => telemetryServiceWriter.Insert
                (
                    It.IsAny<InsertTelemetryItem>()
                )
            )
            .Callback((InsertTelemetryItem insertTelemetryRequest) =>
            {
                insertTelemetryRequestObserved.ConnectionName = insertTelemetryRequest.ConnectionName;
                insertTelemetryRequestObserved.CorrelationId = insertTelemetryRequest.CorrelationId;
                insertTelemetryRequestObserved.DateTimeUTC = insertTelemetryRequest.DateTimeUTC;
                insertTelemetryRequestObserved.Duration = insertTelemetryRequest.Duration;
                insertTelemetryRequestObserved.Request = insertTelemetryRequest.Request;
                insertTelemetryRequestObserved.TelemetryResponseState = insertTelemetryRequest.TelemetryResponseState;
                insertTelemetryRequestObserved.TelemetryType = insertTelemetryRequest.TelemetryType;
            });
            return (telemetryServiceWriterMock, insertTelemetryRequestObserved);
        }

        #endregion
    }
}

