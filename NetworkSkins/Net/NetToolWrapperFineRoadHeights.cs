namespace NetworkSkins.Net
{
    public class NetToolWrapperFineRoadHeights : INetToolWrapper
    {
        public NetInfo GetCurrentPrefab()
        {
            var netTool = ToolsModifierControl.GetCurrentTool<NetToolFine>();
            return netTool?.m_prefab;
        }
    }
}
