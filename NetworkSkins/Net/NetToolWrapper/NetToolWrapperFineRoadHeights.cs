using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public class NetToolWrapperFineRoadHeights : INetToolWrapper
    {
        public NetInfo GetCurrentPrefab()
        {
            var netTool = ToolsModifierControl.GetCurrentTool<NetToolFine>();
            return netTool == null ? null : netTool.m_prefab;
        }
    }
}
