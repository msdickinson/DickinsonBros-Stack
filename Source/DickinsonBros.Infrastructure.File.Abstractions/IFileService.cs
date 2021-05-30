using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.File.Abstractions
{
    public interface IFileService
    {
        Task UpsertFileAsync(string path, string file, Encoding encoding, CancellationToken cancellationToken = default);
        Task UpsertFileAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
        Task<byte[]> LoadFileAsync(string path, CancellationToken cancellationToken = default);
        Task<string> LoadFileAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
        bool FileExists(string path);
        void DeleteFile(string path);
    }
}