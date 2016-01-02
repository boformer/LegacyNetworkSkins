namespace NetworkSkins.Net
{
    public class NetToolWrapperVanilla : INetToolWrapper
    {
        public NetInfo GetCurrentPrefab()
        {
            var netTool = ToolsModifierControl.GetCurrentTool<NetTool>();
            return netTool?.m_prefab;
        }
    }
}
