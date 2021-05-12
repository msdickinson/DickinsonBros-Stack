using DickinsonBros.Core.DateTime;
using System;
using System.Threading.Tasks;

namespace Dickinsonbros.Core.DateTime.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var dateTimeService = new DateTimeService();

            var datetime = dateTimeService.GetDateTimeUTC();
            Console.WriteLine(datetime);

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
