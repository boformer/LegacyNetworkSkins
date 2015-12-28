using ColossalFramework.UI;
using NetworkSkins.UI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class UIPillarOption : UIDropDownOption
    {
        private List<BuildingInfo> availablePillars;

        private PillarType _pillarType;
        public PillarType PillarType {
            get { return _pillarType; }
            set 
            {
                _pillarType = value;
                Description = value.GetDescription();
            }
        }

        protected override bool PopulateDropDown()
        {
            availablePillars = PillarCustomizer.instance.GetAvailablePillars(SelectedPrefab, PillarType);

            if (SelectedPrefab != null && availablePillars != null)
            {
                var defaultPillar = PillarCustomizer.instance.GetDefaultPillar(SelectedPrefab, PillarType);
                var activePillar = PillarCustomizer.instance.GetActivePillar(SelectedPrefab, PillarType);

                DropDown.items = new string[0];

                foreach (var pillar in availablePillars)
                {
                    string itemName = UIUtil.GenerateBeautifiedPrefabName(pillar, defaultPillar);

                    DropDown.AddItem(itemName);

                    if (pillar == activePillar) DropDown.selectedIndex = DropDown.items.Length - 1;
                }

                if (availablePillars.Count >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnSelectionChanged(int index)
        {
            PillarCustomizer.instance.SetPillar(SelectedPrefab, PillarType, availablePillars[index]);
        }
    }
}
