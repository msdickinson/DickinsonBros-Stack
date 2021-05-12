using DickinsonBros.Core.Stopwatch.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Stopwatch
{
    [ExcludeFromCodeCoverage]
    public class StopwatchService : IStopwatchService
    {
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public StopwatchService()
        {
            _stopwatch = new System.Diagnostics.Stopwatch();
        }

        public long ElapsedMilliseconds
        {
            get => _stopwatch.ElapsedMilliseconds;
        }

        public TimeSpan Elapsed
        {
            get => _stopwatch.Elapsed;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }
    }
}
