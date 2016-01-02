using System.Collections.Generic;
using NetworkSkins.UI;

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
                    var itemName = UIUtil.GenerateBeautifiedPrefabName(pillar);
                    itemName = BeautifyNameEvenMore(itemName);
                    if(pillar == defaultPillar) itemName += " (Default)";
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

        private string BeautifyNameEvenMore(string itemName)
        {
            switch (itemName)
            {
                case "Highway Bridge Pillar": return "Medium Concrete Pillar";
                case "Road Small Bridge Pillar": return "Small Double Concrete Pillar";
                case "Medium Bridge Pillar": return "Medium Double Concrete Pillar";
                case "Highway Ramp Pillar": return "Small Concrete Pillar";
                case "Highway Bridge Suspension Pillar": return "Stacked Ornate Concrete Arch";
                case "Large Road Bridge Suspension Pillar": return "Medium Ornate Concrete Arch";
                case "Railway Bridge Pillar": return "Wide Concrete Pillar";
                case "Railway Elevated Pillar": return "Double Steal Beams";
                case "Pedestrian Elevated Pillar": return "Slim Concrete Pillar";
                case "Large Road Bike Bridge Suspension Pillar": return "Wide Ornate Concrete Arch";
                default: return itemName;
            }
        }

        protected override void OnSelectionChanged(int index)
        {
            PillarCustomizer.instance.SetPillar(SelectedPrefab, PillarType, availablePillars[index]);
        }
    }
}
