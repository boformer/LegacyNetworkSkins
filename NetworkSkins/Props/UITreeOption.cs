using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UITreeOption : UINetworkOption
    {
        private UIDropDown treeLeftDropDown;
        private UIDropDown treeRightDropDown;
        protected override void Initialize()
        {
            // Left side tree
            var y = 0f;
            treeLeftDropDown = UIUtil.CreateDropDownWithLabel(this, "Tree Left", y, ParentWidth);
            treeLeftDropDown.items = new string[] { "Palm Tree", "Conifer", "Bush", "None" };
            treeLeftDropDown.selectedIndex = 0;

            y += treeLeftDropDown.height + UINetworkSkinsPanel.PADDING;

            // Right side tree
            treeRightDropDown = UIUtil.CreateDropDownWithLabel(this, "Tree Right", y, ParentWidth);
            treeRightDropDown.items = new string[] { "Palm Tree", "Conifer", "Bush", "None" };
            treeRightDropDown.selectedIndex = 0;
        }

        public override void Populate(NetInfo prefab)
        {
            Debug.Log("TreeOption populate: " + prefab.name);
            //throw new System.NotImplementedException();
        }
    }
}
