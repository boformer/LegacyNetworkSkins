using System.Collections.Generic;
using System.Xml.Serialization;

namespace NetworkSkins.Props
{
    [XmlRoot(ElementName = "pcd")]
    public class PropCustomizerData
    {
        public PropCustomizerData()
        {
            this.SegmentPropDefs = new List<SegmentPropDef>();
        }
        
        [XmlElement(ElementName = "d")]
        public List<SegmentPropDef> SegmentPropDefs { get; set; }
    }
}
