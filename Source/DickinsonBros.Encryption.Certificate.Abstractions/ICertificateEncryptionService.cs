using DickinsonBros.Encryption.Certificate.Abstractions.Models;
using System.Security.Cryptography.X509Certificates;

namespace DickinsonBros.Encryption.Certificate.Abstractions
{
    public interface ICertificateEncryptionService<T> 
    where T : CertificateEncryptionServiceOptionsType
    {
        string Decrypt(byte[] encrypted);
        string Decrypt(string encrypted);
        string Encrypt(string unencrypted);
        byte[] EncryptToByteArray(string unencrypted);
        X509Certificate2 FetchX509Certificate2();
    }
}
