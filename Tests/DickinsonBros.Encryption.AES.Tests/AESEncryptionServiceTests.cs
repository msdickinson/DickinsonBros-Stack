using DickinsonBros.Encryption.AES.Abstractions;
using DickinsonBros.Encryption.AES.Abstractions.Models;
using DickinsonBros.Encryption.AES.Adapter.AspDI.Extensions;
using DickinsonBros.Encryption.AES.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DickinsonBros.Encryption.AES.Tests
{
    [TestClass]
    public class AESEncryptionServiceTests : BaseTest
    {
        [TestMethod]
        public void Encrypt_StringInput_ReturnsExpectedString()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var plainText = "abc123!";
                    var expectedBase64 = "Kn9G12Ye7UP5ar+Bm2D61A==";

                    var uut = serviceProvider.GetRequiredService<IAESEncryptionService<RunnerAESEncryptionServiceOptions>>();
                    var uutConcrete = (AESEncryptionService<RunnerAESEncryptionServiceOptions>)uut;

                    //Act
                    var observed = uutConcrete.Encrypt(plainText);

                    //Assert
                    Assert.AreEqual(expectedBase64, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void EncryptToByteArray_StringInput_ReturnsExpectedByteArray()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var plainText = "abc123!";
                    var expectedBase64 = "Kn9G12Ye7UP5ar+Bm2D61A==";

                    var uut = serviceProvider.GetRequiredService<IAESEncryptionService<RunnerAESEncryptionServiceOptions>>();
                    var uutConcrete = (AESEncryptionService<RunnerAESEncryptionServiceOptions>)uut;

                    //Act
                    var observed = uutConcrete.EncryptToByteArray(plainText);
                    var observedBase64 = Convert.ToBase64String(observed);

                    //Assert
                    Assert.AreEqual(expectedBase64, observedBase64);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Decrypt_WithVaildEncodedByteArray_ReturnsDecryptedString()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var encoded = "Kn9G12Ye7UP5ar+Bm2D61A==";
                    var encodedByteArray = Convert.FromBase64String(encoded);

                    var expectedText = "abc123!";

                    var uut = serviceProvider.GetRequiredService<IAESEncryptionService<RunnerAESEncryptionServiceOptions>>();
                    var uutConcrete = (AESEncryptionService<RunnerAESEncryptionServiceOptions>)uut;

                    //Act
                    var observed = uutConcrete.Decrypt(encodedByteArray);

                    //Assert
                    Assert.AreEqual(expectedText, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void Decrypt_WithVaildEncodedString_ReturnsDecryptedString()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var encoded = "Kn9G12Ye7UP5ar+Bm2D61A==";

                    var expectedText = "abc123!";

                    var uut = serviceProvider.GetRequiredService<IAESEncryptionService<RunnerAESEncryptionServiceOptions>>();
                    var uutConcrete = (AESEncryptionService<RunnerAESEncryptionServiceOptions>)uut;

                    //Act
                    var observed = uutConcrete.Decrypt(encoded);

                    //Assert
                    Assert.AreEqual(expectedText, observed);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            var runnerAESEncryptionServiceOptions = new AESEncryptionServiceOptions<RunnerAESEncryptionServiceOptions>
            {
                InitializationVector = "BqjK8H2y67JBQ/4Zj/7HnQ==",
                Key = "n1kNM3rmrsmacldt1XgIA3i2WlXTvR5aG3qK8Oq6ibA="
            };

            var options = Options.Create(runnerAESEncryptionServiceOptions);
            serviceCollection.AddSingleton(options);
            serviceCollection.AddAESEncryptionService<RunnerAESEncryptionServiceOptions>();

            return serviceCollection;
        }
    }

    public class RunnerAESEncryptionServiceOptions : AESEncryptionServiceOptionsType
    {
    }
}
