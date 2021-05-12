using System;

namespace DickinsonBros.Core.Stopwatch.Abstractions
{
    public interface IStopwatchService
    {
        TimeSpan Elapsed { get; }
        long ElapsedMilliseconds { get; }

        void Reset();
        void Start();
        void Stop();
    }
}
