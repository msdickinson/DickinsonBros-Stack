using DickinsonBros.Encryption.Certificate;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Models;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Core.Encryption.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var certificateEncryptionServiceOptions = new CertificateEncryptionServiceOptions<Configuration>
            {
                ThumbPrint = "D253E5AF8C117821188A097682F3817E16FE9761",
                StoreLocation = "LocalMachine"
            };

            var options = Options.Create(certificateEncryptionServiceOptions);
            var certificateEncryptionService = new CertificateEncryptionService<Configuration>(options);

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

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
