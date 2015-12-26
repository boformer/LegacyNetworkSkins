using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Meshes
{
    public class UIBridgeTypeOption : UINetworkOption
    {
        private UIDropDown bridgeTypeDropDown;
        protected override void Initialize()
        {
            var y = 0f;
            bridgeTypeDropDown = UIUtil.CreateDropDownWithLabel(this, "Bridge Type", y, ParentWidth);
            bridgeTypeDropDown.items = new string[] { "Girder Bridge", "Truss Bridge", "Steel Arch Bridge", "Stone Arch Bridge", "Suspension Bridge" };
            bridgeTypeDropDown.selectedIndex = 0;
        }

        public override void Populate(NetInfo prefab)
        {
            Debug.Log("BridgeTypeOption populate: " + prefab.name);
            //throw new System.NotImplementedException();
        }
    }
}
