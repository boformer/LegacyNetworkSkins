using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UILightOption : UIDropDownOption
    {
        protected override void Initialize() 
        {
            Description = "Street Light";
            base.Initialize();
        }
        
        protected override bool PopulateDropDown()
        {
            DropDown.items = new string[] { "Orange Street Light", "White Street Light", "Chinese Lanterns", "None" };
            DropDown.selectedIndex = 1;
            return true;
        }

        protected override void OnSelectionChanged(int index)
        {
            //throw new System.NotImplementedException();
        }
    }
}
