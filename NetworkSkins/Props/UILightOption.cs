using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UILightOption : UINetworkOption
    {
        private UIDropDown lightDropDown;
        protected override void Initialize()
        {
            var y = 0f;
            lightDropDown = UIUtil.CreateDropDownWithLabel(this, "Street Light", y, ParentWidth);
            lightDropDown.items = new string[] { "Street Light (Yellow)", "Street Light (White)", "Chinese Lanterns", "None" };
            lightDropDown.selectedIndex = 0;
        }

        public override void Populate(NetInfo prefab)
        {
            Debug.Log("LightOption populate: " + prefab.name);
            //throw new System.NotImplementedException();
        }
    }
}
