using DickinsonBros.Core.Stopwatch;
using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.Stopwatch.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var stopwatchService = new StopwatchService();

            Console.WriteLine("Start Timer And Wait 1 Seconds");
            stopwatchService.Start();
            System.Threading.Thread.Sleep(1000);
            stopwatchService.Stop();
            Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

            Console.WriteLine("Start Continue 2 Seconds");
            stopwatchService.Start();
            System.Threading.Thread.Sleep(1000);
            stopwatchService.Stop();
            Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

            Console.WriteLine("Start Timer Reset");
            stopwatchService.Reset();
            Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

            Console.WriteLine("Start Timer And Wait 1 Seconds");
            stopwatchService.Start();
            System.Threading.Thread.Sleep(1000);
            stopwatchService.Stop();
            Console.WriteLine($"ElapsedMilliseconds: {stopwatchService.ElapsedMilliseconds}");

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
