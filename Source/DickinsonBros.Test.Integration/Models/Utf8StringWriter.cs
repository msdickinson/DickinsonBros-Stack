using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace DickinsonBros.Test.Integration.Models
{
    [ExcludeFromCodeCoverage]
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
