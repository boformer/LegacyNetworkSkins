using System;
using System.Collections.Generic;
using System.Globalization;
using NetworkSkins.Net;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class UITreeOption : UIDropDownTextFieldOption
    {
        private List<TreeInfo> _availableTrees;

        private bool ignoreTextFieldChanged = false;

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

                var defaultDistance = PropCustomizer.Instance.GetDefaultTreeDistance(SelectedPrefab, _lanePosition);
                var activeDistance = PropCustomizer.Instance.GetActiveTreeDistance(SelectedPrefab, _lanePosition);
                TextField.text = activeDistance.ToString(CultureInfo.InvariantCulture);
                TextField.tooltip = $"Distance between trees in m (default {defaultDistance})\nValue must be between 1 and 100!";

                if (_availableTrees.Count >= 2)
                {
                    DropDown.Enable();
                }
                else
                {
                    DropDown.Disable();
                }
                return true;
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

        protected override void OnTextChanged(string value)
        {
            if (ignoreTextFieldChanged) return;

            float distance;

            try
            {
                distance = Mathf.Clamp(Convert.ToSingle(value), 1f, 100f);
                PropCustomizer.Instance.SetTreeDistance(SelectedPrefab, _lanePosition, distance);
            }
            catch
            {
                distance = PropCustomizer.Instance.GetActiveTreeDistance(SelectedPrefab, _lanePosition);
            }

            ignoreTextFieldChanged = true;
            TextField.text = distance.ToString(CultureInfo.InvariantCulture);
            ignoreTextFieldChanged = false;
        }
    }
}
