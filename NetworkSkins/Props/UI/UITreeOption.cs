using ColossalFramework.UI;
using NetworkSkins.Net;
using NetworkSkins.UI;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UITreeOption : UIDropDownOption
    {
        private List<TreeInfo> availableTrees;

        private LanePosition _lanePosition;
        public LanePosition LanePosition { 
            get { return _lanePosition; }
            set 
            { 
                _lanePosition = value;
                Description = value.GetDescription() + " Tree";
            } 
        }

        protected override bool PopulateDropDown()
        {
            availableTrees = PropCustomizer.instance.GetAvailableTrees(SelectedPrefab);

            if (SelectedPrefab != null && availableTrees != null && PropCustomizer.instance.HasTrees(SelectedPrefab, LanePosition))
            {
                var defaultTree = PropCustomizer.instance.GetDefaultTree(SelectedPrefab, LanePosition);
                var activeTree = PropCustomizer.instance.GetActiveTree(SelectedPrefab, LanePosition);

                DropDown.items = new string[0];

                foreach (var tree in availableTrees)
                {
                    string itemName = UIUtil.GenerateBeautifiedPrefabName(tree, defaultTree);

                    DropDown.AddItem(itemName);

                    if (tree == activeTree) DropDown.selectedIndex = DropDown.items.Length - 1;
                }

                if (availableTrees.Count >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnSelectionChanged(int index)
        {
            PropCustomizer.instance.SetTree(SelectedPrefab, LanePosition, availableTrees[index]);
        }
    }
}
