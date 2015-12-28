using ColossalFramework.UI;
using NetworkSkins.UI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class UIPillarOption : UINetworkOption
    {
        private bool active = false;
        private NetInfo selectedPrefab;
        private List<BuildingInfo> bridgePillars;
        private List<BuildingInfo> middlePillars;
        
        private UIDropDown bridgePillarDropDown;
        private UIDropDown middlePillarDropDown;
        protected override void Initialize()
        {
            // Bridge Pillar
            var y = 0f;
            bridgePillarDropDown = UIUtil.CreateDropDownWithLabel(this, "Bridge Pillar", y, ParentWidth);
            bridgePillarDropDown.selectedIndex = 0;
            bridgePillarDropDown.eventSelectedIndexChanged += bridgePillarDropDown_eventSelectedIndexChanged;

            y += bridgePillarDropDown.height + UINetworkSkinsPanel.PADDING;

            // Middle Pillar
            middlePillarDropDown = UIUtil.CreateDropDownWithLabel(this, "Middle Pillar", y, ParentWidth);
            middlePillarDropDown.selectedIndex = 0;
            middlePillarDropDown.eventSelectedIndexChanged += middlePillarDropDown_eventSelectedIndexChanged;
        }

        public override void Populate(NetInfo prefab)
        {
            active = false;

            selectedPrefab = prefab;
            if (selectedPrefab == null) return;

            bool visible = PopulateDropDown(PillarType.BRIDGE_PILLAR);
            visible = PopulateDropDown(PillarType.MIDDLE_PILLAR) || visible;

            this.isVisible = visible;

            active = true;
        }

        private bool PopulateDropDown(PillarType type)
        {
            var pillarDropDown = (type == PillarType.BRIDGE_PILLAR ? bridgePillarDropDown : middlePillarDropDown);
            
            var pillars = PillarCustomizer.instance.GetAvailablePillars(selectedPrefab, type);

            if (type == PillarType.BRIDGE_PILLAR) bridgePillars = pillars;
            else middlePillars = pillars;

            if (selectedPrefab != null && pillars != null)
            {
                var defaultPillar = PillarCustomizer.instance.GetDefaultPillar(selectedPrefab, type);
                var activePillar = PillarCustomizer.instance.GetActivePillar(selectedPrefab, type);

                pillarDropDown.items = new string[0];

                foreach (var pillar in pillars)
                {
                    string itemName = UIUtil.GenerateBeautifiedPrefabName(pillar, defaultPillar);

                    pillarDropDown.AddItem(itemName);

                    if (pillar == activePillar) pillarDropDown.selectedIndex = pillarDropDown.items.Length - 1;
                }

                if (pillars.Count >= 2)
                {
                    pillarDropDown.Enable();
                    return true;
                }
            }
            pillarDropDown.Disable();
            return false;
        }



        private void bridgePillarDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            if (!active) return;
            Debug.LogFormat("Changing Bridge Pillars");
            PillarCustomizer.instance.SetPillar(selectedPrefab, PillarType.BRIDGE_PILLAR, bridgePillars[index]);
        }

        private void middlePillarDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            if (!active) return;
            Debug.LogFormat("Changing Middle Pillars");
            PillarCustomizer.instance.SetPillar(selectedPrefab, PillarType.MIDDLE_PILLAR, middlePillars[index]);
        }
    }
}
