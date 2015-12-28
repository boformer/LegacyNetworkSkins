using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.UI;
using NetworkSkins.Net;
using UnityEngine;
using System;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private UINetworkSkinsPanel panel;

        public string Name
        {
            get
            {
                /*
                // Code for GUI debugging
                if (panel == null)
                {
                    panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
                }
                */
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

            if (panel != null)
            {
                UnityEngine.Object.Destroy(panel);
            }
        }

        public static string GetModPath()
        {
            foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                // TODO check for workshop id
                if ((current.name.Contains("NetworkSkins"))) return current.modPath;
            }
            return "";
        }

        public static bool CheckLoadMode(LoadMode mode) 
        {
            return mode == LoadMode.LoadGame || mode == LoadMode.NewGame;
        }
    }
}
