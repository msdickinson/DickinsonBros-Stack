using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration.Runner.NoDI.ExampleTest
{
    public interface IExampleTests
    {
        Task Example_MethodOne_Async(List<string> successLog);
        Task Example_MethodTwo_Async(List<string> successLog);
    }
}