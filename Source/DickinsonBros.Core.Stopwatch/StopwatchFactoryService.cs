using DickinsonBros.Core.Stopwatch.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.Stopwatch
{
    [ExcludeFromCodeCoverage]
    public class StopwatchFactory : IStopwatchFactory
    {
        public IStopwatchService NewStopwatchService()
        {
            return new StopwatchService();
        }
    }
}
