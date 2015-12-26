using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public static class NetUtil
    {
        public static readonly string[] NET_TYPE_NAMES = { "Tunnel", "Ground", "Elevated", "Bridge" };

        public static NetInfo[] GetSubPrefabs(NetInfo prefab)
        {
            NetInfo[] subPrefabs = new NetInfo[NET_TYPE_NAMES.Length];

            if (prefab.m_netAI is TrainTrackAI)
            {
                var netAI = (prefab.m_netAI as TrainTrackAI);
                subPrefabs[(int)NetType.TUNNEL] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.GROUND] = netAI.m_info;
                subPrefabs[(int)NetType.ELEVATED] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.BRIDGE] = netAI.m_bridgeInfo;
            }
            else if (prefab.m_netAI is RoadAI)
            {
                var netAI = (prefab.m_netAI as RoadAI);
                subPrefabs[(int)NetType.TUNNEL] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.GROUND] = netAI.m_info;
                subPrefabs[(int)NetType.ELEVATED] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.BRIDGE] = netAI.m_bridgeInfo;
            }
            else if (prefab.m_netAI is PedestrianPathAI)
            {
                var netAI = (prefab.m_netAI as PedestrianPathAI);
                subPrefabs[(int)NetType.TUNNEL] = netAI.m_tunnelInfo;
                subPrefabs[(int)NetType.GROUND] = netAI.m_info;
                subPrefabs[(int)NetType.ELEVATED] = netAI.m_elevatedInfo;
                subPrefabs[(int)NetType.BRIDGE] = netAI.m_bridgeInfo;
            }

            return subPrefabs;
        }
    }
}
