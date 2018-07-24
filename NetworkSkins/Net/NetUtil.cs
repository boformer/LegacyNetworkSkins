using System.Linq;
using ColossalFramework.Plugins;
using UnityEngine;

namespace NetworkSkins.Net
{
    public static class NetUtil
    {
        public static readonly string[] NET_TYPE_NAMES = { "Tunnel", "Ground", "Elevated", "Bridge" };

        // Get the specific variations of a network for tunnel, ground, elevated and bridge.
        // Each network type has a distint AI, which stores this info.
        // To support other network types (e.g. monorail), you have to add them here.
        // Otherwise the NS settings window will not show up!
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
            else if (prefab.m_netAI is PedestrianWayAI)
            {
                var netAI = (PedestrianWayAI)prefab.m_netAI;
                subPrefabs[(int)NetType.Tunnel] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = netAI.m_info;
                subPrefabs[(int)NetType.Elevated] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = netAI.m_bridgeInfo;
            }
            return subPrefabs;
        }

        // Support for FineRoadHeights and Vanilla NetTool
        // (to support mods which replace the NetTool, currently only vanilla supported)
        public static INetToolWrapper GenerateNetToolWrapper()
        {
            return new NetToolWrapperVanilla();

            // TODO removed FineRoadHeights support remainings!
            /*
            if (!FineRoadHeightsEnabled)
            {
                Debug.Log("Network Skins: FineRoadHeights not detected!");
                return new NetToolWrapperVanilla();
            }
            else
            {
                Debug.Log("Network Skins: FineRoadHeights detected!");
                return new NetToolWrapperFineRoadHeights();
            }
            */
        }

        /*
        private static bool FineRoadHeightsEnabled
        {
            get
            {
                return PluginManager.instance.GetPluginsInfo().Any(mod => (mod.publishedFileID.AsUInt64 == 413678178uL || mod.name.Contains("FineRoadHeights")) && mod.isEnabled);
            }
        }
        */
    }
}
