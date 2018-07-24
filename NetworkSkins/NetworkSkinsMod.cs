using System;
using System.Linq;
using ColossalFramework.Plugins;
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
        private UINetworkSkinsPanel _panel;

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
            if (_panel == null)
            {
                _panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
            }
            _panel.isVisible = true;
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            SegmentDataManager.Instance.OnLevelUnloaded();

            if (_panel != null)
            {
                UnityEngine.Object.Destroy(_panel);
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
            return mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario;
            // TODO support map editor: || mode == LoadMode.LoadMap || mode == LoadMode.NewMap ||
        }
    }
}
