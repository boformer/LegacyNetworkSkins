using System.Collections.Generic;
using NetworkSkins.UI;

namespace NetworkSkins.Props
{
    public class UILightOption : UIDropDownOption
    {
        private List<PropInfo> _availableStreetLights;

        protected override void Initialize() 
        {
            Description = "Street Light";
            base.Initialize();
        }
        
        protected override bool PopulateDropDown()
        {
            _availableStreetLights = PropCustomizer.Instance.GetAvailableStreetLights(SelectedPrefab);

            if (SelectedPrefab != null && _availableStreetLights != null && PropCustomizer.Instance.HasStreetLights(SelectedPrefab))
            {
                var defaultProp = PropCustomizer.Instance.GetDefaultStreetLight(SelectedPrefab);
                var activeProp = PropCustomizer.Instance.GetActiveStreetLight(SelectedPrefab);

                DropDown.items = new string[0];

                foreach (var prop in _availableStreetLights)
                {
                    var itemName = UIUtil.GenerateBeautifiedPrefabName(prop);
                    itemName = BeautifyNameEvenMore(itemName);
                    if (prop == defaultProp) itemName += " (Default)";

                    DropDown.AddItem(itemName);

                    if (prop == activeProp) DropDown.selectedIndex = DropDown.items.Length - 1;
                }

                if (_availableStreetLights.Count >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        private string BeautifyNameEvenMore(string itemName)
        {
            switch (itemName)
            {
                case "New Street Light": return "Cold White Street Light";
                case "New Street Light Highway": return "Warm White Street Light";
                case "New Street Light Small Road": return "Neutral White Street Light";
                case "Avenue Light": return "Sodium-vapor Double Street Light";
                case "Street Lamp #1": return "Modern Street Lamp";
                case "Street Lamp #2": return "Old Street Lamp";
                default: return itemName;
            }
        }

        protected override void OnSelectionChanged(int index)
        {
            PropCustomizer.Instance.SetStreetLight(SelectedPrefab, _availableStreetLights[index]);
        }
    }
}
