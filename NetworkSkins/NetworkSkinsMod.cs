using System;
using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.Data;
using NetworkSkins.Detour;
using NetworkSkins.Net;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
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

            Debug.Log("Begin NetworkSkinsMod.OnLevelLoaded");

            SegmentDataManager.Instance.OnLevelLoaded();

            // Don't load if it's not a game
            if (!CheckLoadMode(mode)) return;

            // GUI
            if (panel == null)
            {
                panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
            }
            panel.isVisible = true;

            Debug.Log("End NetworkSkinsMod.OnLevelLoaded");
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

        public static string AssemblyPath => PluginInfo.modPath;

        private static PluginManager.PluginInfo PluginInfo
        {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (!(instances.FirstOrDefault() is NetworkSkinsMod))
                        {
                            continue;
                        }
                        return item;
                    }
                    catch
                    {

                    }
                }
                throw new Exception("Failed to find Network Skins assembly!");

            }
        }

        public static bool CheckLoadMode(LoadMode mode) 
        {
            return mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.LoadMap || mode == LoadMode.NewMap || mode == LoadMode.NewGameFromScenario;
        }
    }
}
