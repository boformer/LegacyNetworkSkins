using ICities;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        public string Name
        {
            get { return "Pillar Changer"; }//Network Skins
        }
        public string Description
        {
            get { return "Change the pillars of elevated networks. Different stuff coming soon..."; }//Change the visual appearance of roads, train tracks and other networks
        }
    }
}
