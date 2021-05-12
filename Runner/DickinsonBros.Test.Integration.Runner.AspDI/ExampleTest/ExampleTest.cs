using DickinsonBros.Test.Integration.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DickinsonBros.Test.Integration.Runner.AspDI.ExampleTest
{
    [TestAPIAttribute(Name = "ExampleTestClass", Group = "TestGroupOne")]
    public class ExampleTests : IExampleTests
    {
        public async Task Example_MethodOne_Async(List<string> successLog)
        {
            successLog.Add("Step 1 Successful");
            await Task.CompletedTask.ConfigureAwait(false);
        }
        public async Task Example_MethodTwo_Async(List<string> successLog)
        {
            successLog.Add("Step 1 Successful");
            await Task.CompletedTask.ConfigureAwait(false);
            throw new Exception("SampleException");
        }
    }
}
