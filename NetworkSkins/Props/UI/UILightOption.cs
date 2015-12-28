using ColossalFramework.UI;
using NetworkSkins.UI;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UILightOption : UIDropDownOption
    {
        private List<PropInfo> availableStreetLights;

        protected override void Initialize() 
        {
            Description = "Street Light";
            base.Initialize();
        }
        
        protected override bool PopulateDropDown()
        {
            availableStreetLights = PropCustomizer.instance.GetAvailableStreetLights(SelectedPrefab);

            if (SelectedPrefab != null && availableStreetLights != null && PropCustomizer.instance.HasStreetLights(SelectedPrefab))
            {
                var defaultProp = PropCustomizer.instance.GetDefaultStreetLight(SelectedPrefab);
                var activeProp = PropCustomizer.instance.GetActiveStreetLight(SelectedPrefab);

                DropDown.items = new string[0];

                foreach (var prop in availableStreetLights)
                {
                    string itemName = UIUtil.GenerateBeautifiedPrefabName(prop, defaultProp);

                    DropDown.AddItem(itemName);

                    if (prop == activeProp) DropDown.selectedIndex = DropDown.items.Length - 1;
                }

                if (availableStreetLights.Count >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnSelectionChanged(int index)
        {
            PropCustomizer.instance.SetStreetLight(SelectedPrefab, availableStreetLights[index]);
        }
    }
}
