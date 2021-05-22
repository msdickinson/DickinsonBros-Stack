using DickinsonBros.Core.Logger.Abstractions;
using DickinsonBros.Core.Logger.Abstractions.Models;
using DickinsonBros.Core.Stopwatch.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.Logger
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Stopwatch", Group = "Core")]
    public class StopwatchIntegrationTests : IStopwatchIntegrationTests
    {
        public IStopwatchService _stopwatchService;
        public IStopwatchFactory _stopwatchFactory;

        public StopwatchIntegrationTests
        (
            IStopwatchService stopwatchService,
            IStopwatchFactory stopwatchFactory
        )
        {
            _stopwatchService = stopwatchService;
            _stopwatchFactory = stopwatchFactory;
        }

        public async Task StopwatchServiceStartAndStop_Runs_DoesNotThrowNull(List<string> successLog)
        {
            _stopwatchService.Start();
            System.Threading.Thread.Sleep(1000);
            _stopwatchService.Stop();
            Assert.IsTrue(_stopwatchService.ElapsedMilliseconds > 1000 && _stopwatchService.ElapsedMilliseconds < 1100, "Stopwatch is not in expected Range");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task StopwatchServiceStartReset_Runs_DoesNotThrowNull(List<string> successLog)
        {
            _stopwatchService.Start();
            System.Threading.Thread.Sleep(1000);
            _stopwatchService.Stop();
            _stopwatchService.Reset();
            Assert.AreEqual(0, _stopwatchService.ElapsedMilliseconds, "Stopwatch after reset is not 0 ElapsedMilliseconds");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task StopwatchServiceStartStopStartStop_Runs_DoesNotThrowNull(List<string> successLog)
        {
            _stopwatchService.Start();
            System.Threading.Thread.Sleep(500);
            _stopwatchService.Stop();
            _stopwatchService.Start();
            System.Threading.Thread.Sleep(500);
            _stopwatchService.Stop();
            Assert.IsTrue(_stopwatchService.ElapsedMilliseconds > 1000 && _stopwatchService.ElapsedMilliseconds < 1100, "Stopwatch is not in expected Range");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task StopwatchFactoryNewStopwatchService_Runs_DoesNotThrowNull(List<string> successLog)
        {
            var stopwatchOne = _stopwatchFactory.NewStopwatchService();
            var stopwatchTwo = _stopwatchFactory.NewStopwatchService();

            stopwatchOne.Start();
            System.Threading.Thread.Sleep(500);
            stopwatchOne.Stop();

            Assert.AreNotEqual(stopwatchOne, stopwatchTwo, "Both stopwatchs are the same instance");
            Assert.AreNotEqual(stopwatchOne.ElapsedMilliseconds, stopwatchTwo.ElapsedMilliseconds, "Both stopwatchs have the ElapsedMilliseconds");

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
