using ColossalFramework.UI;
using NetworkSkins.UI;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UITreeOption : UINetworkOption
    {
        private bool active = false;
        private NetInfo selectedPrefab;
        private List<TreeInfo> networkTrees;
        
        private UIDropDown treeLeftDropDown;
        private UIDropDown treeMiddleDropDown;
        private UIDropDown treeRightDropDown;
        protected override void Initialize()
        {
            // Left side tree
            var y = 0f;
            treeLeftDropDown = UIUtil.CreateDropDownWithLabel(this, "Tree Left", y, ParentWidth);
            treeLeftDropDown.selectedIndex = 0;
            treeLeftDropDown.eventSelectedIndexChanged += treeLeftDropDown_eventSelectedIndexChanged;

            y += treeLeftDropDown.height + UINetworkSkinsPanel.PADDING;

            // Middle tree
            treeMiddleDropDown = UIUtil.CreateDropDownWithLabel(this, "Tree Middle", y, ParentWidth);
            treeMiddleDropDown.selectedIndex = 0;
            treeMiddleDropDown.eventSelectedIndexChanged += treeMiddleDropDown_eventSelectedIndexChanged;

            y += treeMiddleDropDown.height + UINetworkSkinsPanel.PADDING;

            // Right side tree
            treeRightDropDown = UIUtil.CreateDropDownWithLabel(this, "Tree Right", y, ParentWidth);
            treeRightDropDown.selectedIndex = 0;
            treeRightDropDown.eventSelectedIndexChanged += treeRightDropDown_eventSelectedIndexChanged;
        }

        public override void Populate(NetInfo prefab)
        {
            active = false;

            selectedPrefab = prefab;
            if (selectedPrefab == null) return;

            networkTrees = PropCustomizer.instance.GetAvailableTrees(selectedPrefab);

            bool visible = PopulateDropDown(LanePosition.LEFT, treeLeftDropDown);
            visible = PopulateDropDown(LanePosition.MIDDLE, treeMiddleDropDown) || visible;
            visible = PopulateDropDown(LanePosition.RIGHT, treeRightDropDown) || visible;

            this.isVisible = visible;

            active = true;
        }

        private bool PopulateDropDown(LanePosition type, UIDropDown treeDropDown)
        {
            if (selectedPrefab != null && networkTrees != null && PropCustomizer.instance.HasTrees(selectedPrefab, type))
            {
                var defaultTree = PropCustomizer.instance.GetDefaultTree(selectedPrefab, type);
                var activeTree = PropCustomizer.instance.GetActiveTree(selectedPrefab, type);

                treeDropDown.items = new string[0];

                foreach (var tree in networkTrees)
                {
                    string itemName = UIUtil.GenerateBeautifiedPrefabName(tree, defaultTree);

                    treeDropDown.AddItem(itemName);

                    if (tree == activeTree) treeDropDown.selectedIndex = treeDropDown.items.Length - 1;
                }

                if (networkTrees.Count >= 2)
                {
                    treeDropDown.Enable();
                    return true;
                }
            }
            treeDropDown.Disable();
            return false;
        }

        private void treeLeftDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            if (!active) return;
            Debug.LogFormat("Changing Left Side Road Trees");
            PropCustomizer.instance.SetTree(selectedPrefab, LanePosition.LEFT, networkTrees[index]);
        }

        private void treeMiddleDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            if (!active) return;
            Debug.LogFormat("Changing Middle Road Trees");
            PropCustomizer.instance.SetTree(selectedPrefab, LanePosition.MIDDLE, networkTrees[index]);
        }

        private void treeRightDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            if (!active) return;
            Debug.LogFormat("Changing Right Side Road Trees");
            PropCustomizer.instance.SetTree(selectedPrefab, LanePosition.RIGHT, networkTrees[index]);
        }
    }
}
