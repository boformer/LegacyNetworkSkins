using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Meshes
{
    public class UIBridgeTypeOption : UIDropDownOption
    {
        protected override void Initialize()
        {
            Description = "Bridge Type";
            base.Initialize();
        }

        protected override bool PopulateDropDown()
        {
            DropDown.items = new string[] { "Girder Bridge", "Truss Bridge", "Steel Arch Bridge", "Stone Arch Bridge", "Suspension Bridge" };
            DropDown.selectedIndex = 1;
            return true;
        }

        protected override void OnSelectionChanged(int index)
        {
            //throw new System.NotImplementedException();
        }
    }
}
