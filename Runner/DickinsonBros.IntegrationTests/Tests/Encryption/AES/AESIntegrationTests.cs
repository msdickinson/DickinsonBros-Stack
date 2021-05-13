using DickinsonBros.Encryption.AES.Abstractions;
using DickinsonBros.IntegrationTests.Config;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.AES
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "AES", Group = "Encryption")]
    public class AESIntegrationTests : IAESIntegrationTests
    {
        public IAESEncryptionService<RunnerAESEncryptionServiceOptionsType> _aesEncryptionService;
        public AESIntegrationTests
        (
           IAESEncryptionService<RunnerAESEncryptionServiceOptionsType> aesEncryptionService
        )
        {
            _aesEncryptionService = aesEncryptionService;
        }

        public async Task EncryptAndDecrypt_Runs_MatchsInput(List<string> successLog)
        {
            var input = "Sample123!";

            var encrypted = _aesEncryptionService.Encrypt(input);
            var decrypted = _aesEncryptionService.Decrypt(encrypted);

            Assert.AreEqual(input, decrypted, "Input does not match encrypted and then decrypted string \r\n Input: \r\n{input} \r\nEncrypted: \r\n{encrypted} \r\nDecrypted: \r\n{decrypted}");

            successLog.Add($"Input Matchs encrypted and then decrypted string. \r\nInput:\r\n{input}\r\n \r\nEncrypted:\r\n{encrypted}\r\n \r\nDecrypted:\r\n{decrypted}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task EncryptToByteArrayAndDecrypt_Runs_MatchsInput(List<string> successLog)
        {
            var input = "Sample123!";

            var encrypted = _aesEncryptionService.EncryptToByteArray(input);
            var decrypted = _aesEncryptionService.Decrypt(encrypted);

            Assert.AreEqual(input, decrypted, "Input does not match encrypted and then decrypted string \r\n Input: \r\n{input} \r\nEncrypted: \r\n{encrypted} \r\nDecrypted: \r\n{decrypted}");

            successLog.Add($"Input Matchs encrypted and then decrypted string. \r\nInput:\r\n{input}\r\n \r\nEncrypted:\r\n{System.Text.Encoding.Default.GetString(encrypted)}\r\n \r\nDecrypted:\r\n{decrypted}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
