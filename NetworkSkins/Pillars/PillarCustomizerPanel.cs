using ColossalFramework.Plugins;
using ColossalFramework.UI;
using NetworkSkins.UI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public abstract class PillarCustomizerPanel : UIPanel
    {
        internal NetInfo selectedPrefab;
        internal NetInfo selectedPrefabElevated;
        private List<BuildingInfo> bridgePillars;
        private List<BuildingInfo> middlePillars;

        private bool ignoreEvents = true;

        private UIDropDown bridgePillarDropDown;
        private UIDropDown middlePillarDropDown;
        public override void Start()
        {
            base.Start();
            
            backgroundSprite = "MenuPanel2";
            width = 300;
            height = 175;
            relativePosition = new Vector3(0f, 440f);

            var titlePanel = AddUIComponent<UITitlePanel>();
            titlePanel.Parent = this;
            titlePanel.relativePosition = Vector3.zero;
            titlePanel.IconSprite = "ToolbarIconRoads";
            titlePanel.TitleText = "Elevated Network Pillars";

            var text = AddUIComponent<UILabel>();
            text.relativePosition = new Vector3(10, 50);
            text.text = "Bridge Pillar";

            bridgePillarDropDown = NetworkSkins.UI.UIUtils.CreateDropDown(this);
            bridgePillarDropDown.relativePosition = new Vector3(10, 70);
            bridgePillarDropDown.width = 280;

            bridgePillarDropDown.eventSelectedIndexChanged += (component, index) =>
            {
                if (ignoreEvents) return;
                Debug.LogFormat("Changing Bridge Pillars");
                PillarCustomizer.instance.SetPillar(selectedPrefabElevated, PillarType.BRIDGE_PILLAR, bridgePillars[index]);
            };

            var text2 = AddUIComponent<UILabel>();
            text2.relativePosition = new Vector3(10, 110);
            text2.text = "Middle Pillar";

            middlePillarDropDown = NetworkSkins.UI.UIUtils.CreateDropDown(this);
            middlePillarDropDown.relativePosition = new Vector3(10, 130);
            middlePillarDropDown.width = 280;

            middlePillarDropDown.eventSelectedIndexChanged += (component, index) =>
            {
                if (ignoreEvents) return;
                Debug.LogFormat("Changing Middle Pillars");
                PillarCustomizer.instance.SetPillar(selectedPrefabElevated, PillarType.MIDDLE_PILLAR, middlePillars[index]);
            };
        }        

        internal bool PopulateDropDown(PillarType type) 
        {
            var pillars = PillarCustomizer.instance.GetAvailablePillars(selectedPrefabElevated, type);

            if (type == PillarType.BRIDGE_PILLAR) bridgePillars = pillars;
            else middlePillars = pillars;

            if (selectedPrefabElevated != null && pillars != null) 
            {
                var defaultPillar = PillarCustomizer.instance.GetDefaultPillar(selectedPrefabElevated, type);
                var activePillar = PillarCustomizer.instance.GetActivePillar(selectedPrefabElevated, type);

                var pillarDropDown = (type == PillarType.BRIDGE_PILLAR ? bridgePillarDropDown : middlePillarDropDown);
                
                ignoreEvents = true;

                pillarDropDown.items = new string[0];

                foreach (var pillar in pillars)
                {
                    string itemName = (pillar == null ? "None" : pillar.name);

                    var index1 = itemName.IndexOf('.');
                    if(index1 > -1) itemName = itemName.Substring(index1 + 1);

                    var index2 = itemName.IndexOf("_Data");
                    if(index2 > -1) itemName = itemName.Substring(0, index2);

                    itemName = AddSpacesToSentence(itemName);

                    if(pillar == defaultPillar) itemName += " (Default)";
                    
                    pillarDropDown.AddItem(itemName);

                    if (pillar == activePillar) pillarDropDown.selectedIndex = pillarDropDown.items.Length - 1;
                }

                ignoreEvents = false;

                if (pillars.Count < 2) 
                {
                    pillarDropDown.Disable();
                    return false;
                }
                else
                {
                    pillarDropDown.Enable();
                    return true;
                }
            }
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
    }
}
