using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace DickinsonBros.Test.Integration.Models.Report
{
    [ExcludeFromCodeCoverage]
    public class Times
    {
        [XmlAttribute("creation")]
        public string Creation { get; set; }
        [XmlAttribute("queuing")]
        public string Queuing { get; set; }
        [XmlAttribute("start")]
        public string Start { get; set; }
        [XmlAttribute("finsh")]
        public string Finsh { get; set; }
    }
}
