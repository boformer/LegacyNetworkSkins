using System.Collections.Generic;
using System.Xml.Serialization;

namespace NetworkSkins.Buildings
{
    public class BuildingNetworkSkinsDef
    {
        public List<Building> Buildings { get; set; }

        public BuildingNetworkSkinsDef()
        {
            Buildings = new List<Building>();
        }

        public class Building
        {
            [XmlAttribute]
            public string Name { get; set; }

            public List<NetworkDef> NetworkDefs { get; set; }

            public Building()
            {
                NetworkDefs = new List<NetworkDef>();
            }
        }

        public class NetworkDef
        {
            [XmlAttribute]
            public string Name { get; set; }
            [XmlAttribute]
            public string BridgePillar { get; set; }
            [XmlAttribute]
            public string MiddlePillar { get; set; }
            [XmlAttribute]
            public string TreeLeft { get; set; }
            [XmlAttribute]
            public string TreeMiddle { get; set; }
            [XmlAttribute]
            public string TreeRight { get; set; }
            [XmlAttribute]
            public string StreetLight { get; set; }
        }
    }
}