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

        internal bool PopulateDropDown(PillarType type)
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
                    string itemName = (pillar == null ? "None" : pillar.name);

                    var index1 = itemName.IndexOf('.');
                    if (index1 > -1) itemName = itemName.Substring(index1 + 1);

                    var index2 = itemName.IndexOf("_Data");
                    if (index2 > -1) itemName = itemName.Substring(0, index2);

                    itemName = AddSpacesToSentence(itemName);

                    if (pillar == defaultPillar) itemName += " (Default)";

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

        string AddSpacesToSentence(string text)
        {
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
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
