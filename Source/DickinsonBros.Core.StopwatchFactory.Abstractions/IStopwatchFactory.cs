using DickinsonBros.Core.Stopwatch.Abstractions;

namespace DickinsonBros.Core.StopwatchFactory.Abstractions
{
    public interface IStopwatchFactory
    {
        IStopwatchService NewStopwatchService();
    }
}
