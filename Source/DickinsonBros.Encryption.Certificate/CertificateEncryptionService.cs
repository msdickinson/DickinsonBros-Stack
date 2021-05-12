using DickinsonBros.Encryption.Certificate.Abstractions;
using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using DickinsonBros.Encryption.Certificate.Models;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DickinsonBros.Encryption.Certificate
{
    [ExcludeFromCodeCoverage]
    public class CertificateEncryptionService<T> : ICertificateEncryptionService<T>
    where T : CertificateEncryptionServiceOptionsType
    {
        internal readonly string _thumbPrint;
        internal readonly StoreLocation _storeLocation;

        public CertificateEncryptionService(IOptions<CertificateEncryptionServiceOptions<T>> certificateEncryptionOptions)
        {
            _thumbPrint = certificateEncryptionOptions.Value.ThumbPrint;
            _storeLocation = certificateEncryptionOptions.Value.StoreLocation == "LocalMachine"
                                ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;

        }

        public string Decrypt(string encrypted)
        {
            return Decrypt(Convert.FromBase64String(encrypted));
        }

        public string Decrypt(byte[] encrypted)
        {
            try
            {
                using var x509Store = new X509Store(StoreName.My, _storeLocation);
                x509Store.Open(OpenFlags.ReadOnly);
                var certificateCollection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, _thumbPrint, false);
                if (certificateCollection.Count > 0)
                {
                    var certificate = certificateCollection[0];
                    using var rsaPrivateKey = certificate.GetRSAPrivateKey();
                    return Encoding.ASCII.GetString(rsaPrivateKey.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1));
                }
                else
                {
                    throw new Exception($"No certificate found for Thumbprint {_thumbPrint} in location {_storeLocation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled exception. Thumbprint: {_thumbPrint}, Location: {_storeLocation}", ex);
            }
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(EncryptToByteArray(unencrypted));
        }

        public byte[] EncryptToByteArray(string unencrypted)
        {
            try
            {
                using var x509Store = new X509Store(StoreName.My, _storeLocation);
                x509Store.Open(OpenFlags.ReadOnly);
                var certificateCollection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, _thumbPrint, false);
                if (certificateCollection.Count > 0)
                {
                    var certificate = certificateCollection[0];
                    using RSA rsa = certificate.GetRSAPublicKey();
                    byte[] bytestodecrypt = Encoding.UTF8.GetBytes(unencrypted);

                    return rsa.Encrypt(bytestodecrypt, RSAEncryptionPadding.Pkcs1);
                }
                else
                {
                    throw new Exception($"No certificate found for Thumbprint {_thumbPrint} in location {_storeLocation}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled exception. Thumbprint: {_thumbPrint}, Location: {_storeLocation}", ex);
            }
        }

        public X509Certificate2 FetchX509Certificate2()
        {
            try
            {
                using var x509Store = new X509Store(StoreName.My, _storeLocation);
                x509Store.Open(OpenFlags.ReadOnly);
                var certificateCollection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, _thumbPrint, false);
                if (certificateCollection.Count > 0)
                {
                    return certificateCollection[0];
                }
                else
                {
                    throw new Exception($"No certificate found for Thumbprint {_thumbPrint} in location {_thumbPrint}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unhandled exception. Thumbprint: {_thumbPrint}, Location: {_thumbPrint}", ex);
            }
        }
    }
}
