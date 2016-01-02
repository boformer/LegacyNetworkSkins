using System.Linq;
using ColossalFramework.Plugins;

namespace NetworkSkins.Net
{
    public static class NetUtil
    {
        public static readonly string[] NET_TYPE_NAMES = { "Tunnel", "Ground", "Elevated", "Bridge" };

        public static NetInfo[] GetSubPrefabs(NetInfo prefab)
        {
            var subPrefabs = new NetInfo[NET_TYPE_NAMES.Length];

            if (prefab.m_netAI is TrainTrackAI)
            {
                var netAI = (TrainTrackAI) prefab.m_netAI;
                subPrefabs[(int)NetType.Tunnel] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = netAI.m_info;
                subPrefabs[(int)NetType.Elevated] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = netAI.m_bridgeInfo;
            }
            else if (prefab.m_netAI is RoadAI)
            {
                var netAI = (RoadAI) prefab.m_netAI;
                subPrefabs[(int)NetType.Tunnel] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = netAI.m_info;
                subPrefabs[(int)NetType.Elevated] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = netAI.m_bridgeInfo;
            }
            else if (prefab.m_netAI is PedestrianPathAI)
            {
                var netAI = (PedestrianPathAI) prefab.m_netAI;
                subPrefabs[(int)NetType.Tunnel] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = netAI.m_info;
                subPrefabs[(int)NetType.Elevated] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = netAI.m_bridgeInfo;
            }

            return subPrefabs;
        }

        // Support for FineRoadHeights and Vanilla NetTool
        public static INetToolWrapper GenerateNetToolWrapper()
        {
            if (!FineRoadHeightsEnabled)
            {
                return new NetToolWrapperVanilla();
            }
            else
            {
                return new NetToolWrapperFineRoadHeights();
            }
        }

        private static bool FineRoadHeightsEnabled
        {
            get
            {
                return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == 413678178uL || mod.name.Contains("FineRoadHeights")) && mod.isEnabled);
            }
        }
    }
}
