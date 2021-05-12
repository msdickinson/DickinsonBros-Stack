namespace DickinsonBros.Core.Stopwatch.Abstractions
{
    public interface IStopwatchFactory
    {
        IStopwatchService NewStopwatchService();
    }
}
