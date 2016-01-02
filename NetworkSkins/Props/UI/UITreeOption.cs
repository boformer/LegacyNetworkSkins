using System.Collections.Generic;
using NetworkSkins.Net;
using NetworkSkins.UI;

namespace NetworkSkins.Props
{
    public class UITreeOption : UIDropDownOption
    {
        private List<TreeInfo> _availableTrees;

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
            _availableTrees = PropCustomizer.Instance.GetAvailableTrees(SelectedPrefab);

            if (SelectedPrefab != null && _availableTrees != null && PropCustomizer.Instance.HasTrees(SelectedPrefab, LanePosition))
            {
                var defaultTree = PropCustomizer.Instance.GetDefaultTree(SelectedPrefab, LanePosition);
                var activeTree = PropCustomizer.Instance.GetActiveTree(SelectedPrefab, LanePosition);

                DropDown.items = new string[0];

                foreach (var tree in _availableTrees)
                {
                    var itemName = UIUtil.GenerateBeautifiedPrefabName(tree);
                    itemName = BeautifyNameEvenMore(itemName);
                    if (tree == defaultTree) itemName += " (Default)";
                    DropDown.AddItem(itemName);

                    if (tree == activeTree) DropDown.selectedIndex = DropDown.items.Length - 1;
                }

                if (_availableTrees.Count >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        private string BeautifyNameEvenMore(string itemName) 
        {
            switch(itemName)                   
            {
                case "Cherry Tree01": return "Cherry Tree";
                case "Tree with Leaves": return "Small Oak";
                case "Tree with Leaves #2": return "Large Oak";
                case "Oak": return "Random Oak";
                case "Conifer #2": return "Small Conifer";
                case "Conife": return "Medium Conifer";
                case "Wild Conifer": return "Large Conifer";
                case "Alder #2": return "Alder";
                default: return itemName;
            }          
        }

        protected override void OnSelectionChanged(int index)
        {
            PropCustomizer.Instance.SetTree(SelectedPrefab, LanePosition, _availableTrees[index]);
        }
    }
}
