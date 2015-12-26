using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class UIPillarOption : UINetworkOption
    {
        private bool active = false;
        
        private UIDropDown bridgePillarDropDown;
        private UIDropDown middlePillarDropDown;
        protected override void Initialize()
        {
            // Bridge Pillar
            var y = 0f;
            bridgePillarDropDown = UIUtil.CreateDropDownWithLabel(this, "Bridge Pillar", y, ParentWidth);
            bridgePillarDropDown.items = new string[] { "Pillar A", "Pillar B", "Pillar C" };
            bridgePillarDropDown.selectedIndex = 0;
            bridgePillarDropDown.eventSelectedIndexChanged += bridgePillarDropDown_eventSelectedIndexChanged;

            y += bridgePillarDropDown.height + UINetworkSkinsPanel.PADDING;

            // Middle Pillar
            middlePillarDropDown = UIUtil.CreateDropDownWithLabel(this, "Middle Pillar", y, ParentWidth);
            middlePillarDropDown.items = new string[] { "Pillar D", "Pillar E", "Pillar F" };
            middlePillarDropDown.selectedIndex = 0;
            middlePillarDropDown.eventSelectedIndexChanged += middlePillarDropDown_eventSelectedIndexChanged;
        }

        public override void Populate(NetInfo prefab)
        {
            active = false;

            if (prefab.m_netAI is TrainTrackBridgeAI) 
            {
                var netAI = prefab.m_netAI as TrainTrackBridgeAI;

                middlePillarDropDown.isVisible = netAI.m_doubleLength;


            }
            else if(prefab.m_netAI is )



            active = true;
        }

        private void bridgePillarDropDown_eventSelectedIndexChanged(UIComponent component, int value)
        {
            if (!active) return;
        }

        private void middlePillarDropDown_eventSelectedIndexChanged(UIComponent component, int value)
        {
            if (!active) return;
        }
    }
}
