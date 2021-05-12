using DickinsonBros.Core.Correlation.Abstractions;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Correlation.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            var correlationService = new CorrelationService();

            Console.WriteLine($"This shows that if a async method changes the value, it will stay changed in that context");
            Console.WriteLine($"but will not change the outer context value");
            Console.WriteLine($"");

            correlationService.CorrelationId = "100";
            Console.WriteLine($"CorrelationId Before Run: {correlationService.CorrelationId}");

            await ModifyCorrelationIdAsync("200", correlationService).ConfigureAwait(false);
            Console.WriteLine($"CorrelationId After Run: {correlationService.CorrelationId}");
        }

        public async Task ModifyCorrelationIdAsync(string updateValue, ICorrelationService correlationService)
        {
            Console.WriteLine($"CorrelationId Before Modify: {correlationService.CorrelationId}");
            correlationService.CorrelationId = updateValue;
            Console.WriteLine($"CorrelationId After Modify: {correlationService.CorrelationId}");

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
