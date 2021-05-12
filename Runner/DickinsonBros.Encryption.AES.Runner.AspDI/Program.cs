using BaseRunner;
using DickinsonBros.Encryption.AES.Abstractions;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.AES.Runner.AspDI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.AES.Runner.AspDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                var serviceCollection = ConfigureServices();

                //Start application
                using var provider = serviceCollection.BuildServiceProvider();
                var aesEncryptionService = provider.GetRequiredService<IAESEncryptionService<RunnerAESEncryptionServiceOptions>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run application
                var encryptedString = aesEncryptionService.Encrypt("Sample123!");
                var decryptedString = aesEncryptionService.Decrypt(encryptedString);
                var encryptedByteArray = aesEncryptionService.EncryptToByteArray("Sample123!");
                var decryptedStringFromByteArray = aesEncryptionService.Decrypt(encryptedByteArray);
                Console.WriteLine(
            $@"Encrypted String
{ encryptedString }

Decrypted string
{ decryptedString }

Encrypted To ByteArray
{  Encoding.UTF8.GetString(encryptedByteArray) }

Decrypted String
{ decryptedStringFromByteArray }
");

                //Stop application
                hostApplicationLifetime.StopApplication();
                provider.ConfigureAwait(true);
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private IServiceCollection ConfigureServices()
        {
            var configuration = BaseRunnerSetup.FetchConfiguration();
            var serviceCollection = BaseRunnerSetup.InitializeDependencyInjection(configuration);

            serviceCollection.AddAESEncryptionService<RunnerAESEncryptionServiceOptions>();

            return serviceCollection;
        }
    }
}
