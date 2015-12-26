using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public class NetToolWrapperVanilla : INetToolWrapper
    {
        public NetInfo GetCurrentPrefab()
        {
            var netTool = ToolsModifierControl.GetCurrentTool<NetTool>();
            return netTool == null ? null : netTool.m_prefab;
        }
    }
}
