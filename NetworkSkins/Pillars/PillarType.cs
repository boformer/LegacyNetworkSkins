using System;
namespace NetworkSkins.Pillars
{
    public enum PillarType
    {
        BRIDGE_PILLAR,
        MIDDLE_PILLAR
    }

    public static class PillarTypeExtensions
    {
        public static string GetDescription(this PillarType type) 
        {
            switch (type)
            {
                case PillarType.BRIDGE_PILLAR: return "Bridge Pillar";
                case PillarType.MIDDLE_PILLAR: return "Middle Pillar";
                default: throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
