using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.Test.Integration.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class Execution
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }
}
