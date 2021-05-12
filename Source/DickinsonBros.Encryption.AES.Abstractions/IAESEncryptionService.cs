using DickinsonBros.Encryption.AES.Abstractions.Models;

namespace DickinsonBros.Encryption.AES.Abstractions
{
    public interface IAESEncryptionService<T>
    where T : AESEncryptionServiceOptionsType
    {
        string Decrypt(byte[] encrypted);
        string Decrypt(string encrypted);
        string Encrypt(string unencrypted);
        byte[] EncryptToByteArray(string unencrypted);
    }
}
