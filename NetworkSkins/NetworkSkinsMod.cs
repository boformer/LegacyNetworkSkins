using ColossalFramework.Plugins;
using ColossalFramework.Steamworks;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.Data;
using NetworkSkins.Detour;
using NetworkSkins.Net;
using NetworkSkins.UI;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private const ulong workshopId = 543722850UL;

        private UINetworkSkinsPanel panel;

        public string Name => "Network Skins";
        public string Description => "Change the visual appearance of roads, train tracks and other networks";

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            RenderManagerDetour.Deploy();
            NetManagerDetour.Deploy();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            SegmentDataManager.Instance.OnLevelLoaded();

            // Don't load if it's not a game
            if (!CheckLoadMode(mode)) return;

            // GUI
            if (panel == null)
            {
                panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
            }
            panel.isVisible = true;
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            SegmentDataManager.Instance.OnLevelUnloaded();

            if (panel != null)
            {
                UnityEngine.Object.Destroy(panel);
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();

            RenderManagerDetour.Revert();
            NetManagerDetour.Revert();
        }

        public static string GetModPath()
        {
            foreach (var current in PluginManager.instance.GetPluginsInfo())
            {
                if (current.publishedFileID.AsUInt64 == workshopId || current.name.Contains("NetworkSkins")) return current.modPath;
            }
            return "";
        }

        public static bool CheckLoadMode(LoadMode mode) 
        {
            return mode == LoadMode.LoadGame || mode == LoadMode.NewGame;
        }
    }
}
