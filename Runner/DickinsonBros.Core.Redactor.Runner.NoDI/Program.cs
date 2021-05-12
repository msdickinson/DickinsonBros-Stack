using DickinsonBros.Core.Redactor.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Redactor.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var sampleContract = new SampleContract
            {
                Password = "SamplePassword",
                Username = "SampleUsername"
            };

            var redactorServiceOptions = new RedactorServiceOptions
            {
                PropertiesToRedact = new string[]
                {
                    "Password"
                },
                RegexValuesToRedact = new string[]
                {
                }
            };

            var options = Options.Create(redactorServiceOptions);
            var redactorService = new RedactorService(options);

            var input =
@"{
""Password"": ""password""
}";
            Console.WriteLine($"Raw Json: \r\n {input}");
            Console.WriteLine($"Redacted Json: \r\n { redactorService.Redact(input)}");

            Console.WriteLine($"Sample Model \r\n { System.Text.Json.JsonSerializer.Serialize(sampleContract)}");
            Console.WriteLine($"Redacted Model: \r\n { redactorService.Redact(sampleContract)}");

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }

    public class SampleContract
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
