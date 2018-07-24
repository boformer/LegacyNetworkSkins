namespace NetworkSkins.Net
{
    /// <summary>
    /// Abstraction of NetTool to support mods which replace the NetTool.
    /// The mod which does that (FineRoadHeights) was abandoned a long time ago,
    /// so this is no longer in use. There is only one used abstraction for the vanilla NetTool.
    /// </summary>

    public interface INetToolWrapper
    {
        NetInfo GetCurrentPrefab();
    }
}
