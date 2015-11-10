using ICities;

namespace NetworkSkins
{
    public class NetworkSkinsMod : IUserMod
    {
        public string Name
        {
            get { return "Network Skins"; }
        }
        public string Description
        {
            get { return "Change the visual appearance of roads, train tracks and other networks"; }
        }
    }
}
