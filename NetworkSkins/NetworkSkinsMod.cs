using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.UI;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private UINetworkSkinsPanel panel;

        public string Name
        {
            get
            {
                if (panel == null)
                {
                    panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
                }
                return "Network Skins";
            }
        }
        public string Description
        {
            get { return "Change the visual appearance of roads, train tracks and other networks"; }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

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

            if (panel != null)
            {
                Object.Destroy(panel);
            }
        }

        // Support for FineRoadHeights and Vanilla NetTool
        public static INetToolWrapper GenerateNetToolWrapper()
        {
            if (!FineRoadHeightsEnabled)
            {
                return new NetToolWrapperVanilla();
            }
            else
            {
                return new NetToolWrapperFineRoadHeights();
            }
        }

        public static string GetModPath()
        {
            foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                // TODO check for workshop id
                if ((current.name.Contains("Network Skins"))) return current.modPath;
            }
            return null;
        }

        private static bool FineRoadHeightsEnabled
        {
            get
            {
                foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
                {
                    if ((current.publishedFileID.AsUInt64 == 413678178uL || current.name.Contains("Fine Road Heights")) && current.isEnabled) return true;
                }
                return false;
            }
        }
    }
}
