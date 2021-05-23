using DickinsonBros.Core.Telemetry;
using DickinsonBros.Core.Telemetry.Abstractions;
using DickinsonBros.Core.Telemetry.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DickinsonBros.Core.Redactor.Tests
{
    [TestClass]
    public class TelemetryWriterServiceTests : BaseTest
    {
        [TestMethod]
        public void Insert_Runs_NewTelemetryEventCalled()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>(); 
                    var telemetryItem = new InsertTelemetryItem()
                    {
                        DateTimeUTC = System.DateTime.UtcNow,
                        ConnectionName = "SampleConnectionName",
                        Duration = TimeSpan.FromSeconds(1),
                        SignalRequest = "SampleSignalRequest",
                        SignalResponse = "SampleSignalResponse",
                        TelemetryResponseState = TelemetryResponseState.Successful,
                        TelemetryType = TelemetryType.Application
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;
                    uut.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert
                    Assert.AreEqual(1, telemetryItems.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_NullInput_ThrowsException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>();
                    var telemetryItem = (InsertTelemetryItem)null;

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;
                    uut.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_ConnecitonNameNullOrWhiteSpace_ThrowsException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>(); var telemetryItem = new InsertTelemetryItem()
                    {
                        DateTimeUTC = System.DateTime.UtcNow,
                        ConnectionName = "",
                        Duration = TimeSpan.FromSeconds(1),
                        SignalRequest = "SampleSignalRequest",
                        SignalResponse = "SampleSignalResponse",
                        TelemetryResponseState = TelemetryResponseState.Successful,
                        TelemetryType = TelemetryType.Application
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;
                    uut.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_SignalRequestNullOrWhiteSpace_ThrowsException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>(); var telemetryItem = new InsertTelemetryItem()
                    {
                        DateTimeUTC = System.DateTime.UtcNow,
                        ConnectionName = "SampleConnectionName",
                        Duration = TimeSpan.FromSeconds(1),
                        SignalRequest = "",
                        SignalResponse = "SampleSignalResponse",
                        TelemetryResponseState = TelemetryResponseState.Successful,
                        TelemetryType = TelemetryType.Application
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;
                    uut.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Date Expected to be set")]
        public void Insert_DefaultDateTimeUTC_ThrowsException()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>();
                    var telemetryItem = new InsertTelemetryItem()
                    {
                        ConnectionName = "SampleConnectionName",
                        Duration = TimeSpan.FromSeconds(1),
                        SignalRequest = "SampleSignalRequest",
                        SignalResponse = "SampleSignalResponse",
                        TelemetryResponseState = TelemetryResponseState.Successful,
                        TelemetryType = TelemetryType.Application
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;
                    uut.NewTelemetryEvent += (telemetryItem) => { telemetryItems.Add(telemetryItem); };

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert
                    Assert.AreEqual(1, telemetryItems.Count);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Insert_NoEventListeners_DoesNotThrow()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var telemetryItems = new List<TelemetryItem>();
                    var telemetryItem = new InsertTelemetryItem()
                    {
                        DateTimeUTC = System.DateTime.UtcNow,
                        ConnectionName = "SampleConnectionName",
                        Duration = TimeSpan.FromSeconds(1),
                        SignalRequest = "SampleSignalRequest",
                        SignalResponse = "SampleSignalResponse",
                        TelemetryResponseState = TelemetryResponseState.Successful,
                        TelemetryType = TelemetryType.Application
                    };

                    var uut = serviceProvider.GetRequiredService<ITelemetryWriterService>();
                    var uutConcrete = (TelemetryWriterService)uut;

                    //Act
                    uutConcrete.Insert(telemetryItem);

                    //Assert

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            var telemetryWriterServiceOptions = new TelemetryWriterServiceOptions
            {
                ApplicationName = "SampleApplicationName"
            };
            var options = Options.Create(telemetryWriterServiceOptions);
            serviceCollection.AddSingleton(options);

            serviceCollection.AddSingleton<ITelemetryWriterService, TelemetryWriterService>();

            return serviceCollection;
        }
    }
}