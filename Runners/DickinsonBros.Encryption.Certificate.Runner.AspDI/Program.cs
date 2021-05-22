using BaseRunner;
using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Adapter.AspDI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.Certificate.Runner.AspDI
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
                var certificateEncryptionService = provider.GetRequiredService<ICertificateEncryptionService<Configuration>>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                //Run application
                var encryptedString = certificateEncryptionService.Encrypt("Sample123!");
                var decryptedString = certificateEncryptionService.Decrypt(encryptedString);
                var encryptedByteArray = certificateEncryptionService.EncryptToByteArray("Sample123!");
                var decryptedStringFromByteArray = certificateEncryptionService.Decrypt(encryptedByteArray);
                Console.WriteLine(
    $@"CertificateEncryptionService<Develop>
Encrypted String
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

            serviceCollection.AddCertificateEncryptionService<Configuration>();

            return serviceCollection;
        }
    }
}
