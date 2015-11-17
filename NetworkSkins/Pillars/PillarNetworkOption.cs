using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class PillarNetworkOption : UINetworkOption
    {
        private UIDropDown bridgePillarDropDown;
        private UIDropDown middlePillarDropDown;
        protected override void Initialize()
        {
            bridgePillarDropDown = UIUtil.CreateDropDown(this);
            bridgePillarDropDown.relativePosition = new Vector3(0, 70);
            bridgePillarDropDown.width = ParentWidth;
            bridgePillarDropDown.items = new string[] { "Pillar A", "Pillar B", "Pillar C" };

            middlePillarDropDown = UIUtil.CreateDropDown(this);
            middlePillarDropDown.relativePosition = new Vector3(0, 110);
            middlePillarDropDown.width = ParentWidth;
        }

        public override void Populate(NetInfo prefab)
        {
            throw new System.NotImplementedException();
        }
    }
}
