using DickinsonBros.Encryption.JWT.Abstractions.Models;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Encryption.JWT.Models
{
    [ExcludeFromCodeCoverage]
    public class JWTEncryptionServiceOptions<T, U> : JWTEncryptionServiceOptions
    where T : JWTEncryptionServiceOptionsType
    {

    }
}
