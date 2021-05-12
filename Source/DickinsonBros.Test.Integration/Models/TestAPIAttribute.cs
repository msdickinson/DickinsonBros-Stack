using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Test.Integration.Models
{
    [ExcludeFromCodeCoverage]
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct,
                       AllowMultiple = true)
    ]
    public class TestAPIAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Group { get; set; }
    }
}
