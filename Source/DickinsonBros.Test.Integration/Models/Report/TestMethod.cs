using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.Test.Integration.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class TestMethod
    {
        [XmlAttribute("codeBase")]
        public string CodeBase { get; set; }

        [XmlAttribute("adapterTypeName")]
        public string AdapterTypeName { get; set; }

        [XmlAttribute("className")]
        public string ClassName { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
