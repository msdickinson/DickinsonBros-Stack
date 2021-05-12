using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.Guid.Runner.NoDi
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var guidService = new GuidService();

            Console.WriteLine(guidService.NewGuid());

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
