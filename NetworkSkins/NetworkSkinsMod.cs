using ColossalFramework.UI;
using ICities;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private UINetworkSkinsPanel panel;
        
        public string Name
        {
            get {
                if (panel == null)
                {
                    panel = UIView.GetAView().AddUIComponent(typeof(UINetworkSkinsPanel)) as UINetworkSkinsPanel;
                }
                return "Network Skins"; }
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
    }
}
