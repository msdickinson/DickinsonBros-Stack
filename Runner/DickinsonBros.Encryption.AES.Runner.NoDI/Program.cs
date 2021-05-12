using DickinsonBros.Encryption.AES.Models;
using DickinsonBros.Encryption.AES.Runner.NoDI.Models;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.Encryption.AES.Runner.NoDI
{
    class Program
    {
        async static Task Main()
        {
            await new Program().DoMain().ConfigureAwait(false);
        }
        async Task DoMain()
        {
            var certificateEncryptionServiceOptions = new AESEncryptionServiceOptions<RunnerAESEncryptionServiceOptions>
            {
                InitializationVector = "BqjK8H2y67JBQ/4Zj/7HnQ==",
                Key = "n1kNM3rmrsmacldt1XgIA3i2WlXTvR5aG3qK8Oq6ibA="
            };

            var options = Options.Create(certificateEncryptionServiceOptions);
            var aesEncryptionService = new AESEncryptionService<RunnerAESEncryptionServiceOptions>(options);

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

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
