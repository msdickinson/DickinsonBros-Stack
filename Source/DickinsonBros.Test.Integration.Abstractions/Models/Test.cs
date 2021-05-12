using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DickinsonBros.Test.Integration.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class Test
    {
        public MethodInfo MethodInfo { get; set; }
        public object TestClass { get; set; }
        public string TestsName { get; set; }
        public string TestGroup { get; set; }
    }
}
