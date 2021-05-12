using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests
{
    public interface IRunner
    {
        Task<string> RunAsync();
    }
}