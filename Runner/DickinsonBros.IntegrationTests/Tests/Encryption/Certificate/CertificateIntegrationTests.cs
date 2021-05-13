using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Encryption.Certificate
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "Certificate", Group = "Encryption")]
    public class CertificateIntegrationTests : ICertificateIntegrationTests
    {
        public ICertificateEncryptionService<Configuration> _certificateEncryptionService;

        public CertificateIntegrationTests
        (
            ICertificateEncryptionService<Configuration> certificateEncryptionService
        )
        {
            _certificateEncryptionService = certificateEncryptionService;
        }

        public async Task EncryptAndDecrypt_Runs_MatchsInput(List<string> successLog)
        {
            var input = "Sample123!";

            var encrypted = _certificateEncryptionService.Encrypt(input);
            var decrypted = _certificateEncryptionService.Decrypt(encrypted);

            Assert.AreEqual(input, decrypted, "Input does not match encrypted and then decrypted string \r\n Input: \r\n{input} \r\nEncrypted: \r\n{encrypted} \r\nDecrypted: \r\n{decrypted}");

            successLog.Add($"Input Matchs encrypted and then decrypted string. \r\nInput:\r\n{input}\r\n \r\nEncrypted:\r\n{encrypted}\r\n \r\nDecrypted:\r\n{decrypted}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task EncryptToByteArrayAndDecrypt_Runs_MatchsInput(List<string> successLog)
        {
            var input = "Sample123!";

            var encrypted = _certificateEncryptionService.EncryptToByteArray(input);
            var decrypted = _certificateEncryptionService.Decrypt(encrypted);

            Assert.AreEqual(input, decrypted, "Input does not match encrypted and then decrypted string \r\n Input: \r\n{input} \r\nEncrypted: \r\n{encrypted} \r\nDecrypted: \r\n{decrypted}");

            successLog.Add($"Input Matchs encrypted and then decrypted string. \r\nInput:\r\n{input}\r\n \r\nEncrypted:\r\n{System.Text.Encoding.Default.GetString(encrypted)}\r\n \r\nDecrypted:\r\n{decrypted}\r\n");

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
