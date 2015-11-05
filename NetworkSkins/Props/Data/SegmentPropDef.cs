using System.ComponentModel;
using System.Xml.Serialization;

namespace NetworkSkins.Props
{
    public class SegmentPropDef
    {
        [XmlAttribute("t0"), DefaultValue(null)]
        public string Tree0 { get; set; }

        [XmlAttribute("t1"), DefaultValue(null)]
        public string Tree1 { get; set; }

        [XmlAttribute("s"), DefaultValue(null)]
        public string StreetLight { get; set; }

        [XmlAttribute("d"), DefaultValue(null)]
        public bool? DisplayDecals { get; set; }
    }
}
