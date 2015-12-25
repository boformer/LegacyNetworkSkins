using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class PillarNetworkOption : UINetworkOption
    {
        private const float LABEL_WIDTH = .30f;
        private const float COLUMN_PADDING = 5f;
        
        private UIDropDown bridgePillarDropDown;
        private UIDropDown middlePillarDropDown;
        protected override void Initialize()
        {
            var labelWidth = Mathf.Round(ParentWidth * LABEL_WIDTH);

            var y = 0f;

            // Bridge Pillar
            bridgePillarDropDown = UIUtil.CreateDropDown(this);
            bridgePillarDropDown.relativePosition = new Vector3(labelWidth + COLUMN_PADDING, y);
            bridgePillarDropDown.width = ParentWidth - labelWidth - COLUMN_PADDING;
            bridgePillarDropDown.items = new string[] { "Pillar A", "Pillar B", "Pillar C" };

            AddLabel("Bridge Pillar", labelWidth, bridgePillarDropDown.height, y);

            y += bridgePillarDropDown.height + UINetworkSkinsPanel.PADDING;

            // Middle Pillar
            middlePillarDropDown = UIUtil.CreateDropDown(this);
            middlePillarDropDown.relativePosition = new Vector3(labelWidth + COLUMN_PADDING, y);
            middlePillarDropDown.width = ParentWidth - labelWidth - COLUMN_PADDING;
            middlePillarDropDown.items = new string[] { "Pillar D", "Pillar E", "Pillar F" };

            AddLabel("Middle Pillar", labelWidth, middlePillarDropDown.height, y);
        }

        private void AddLabel(string text, float width, float dropDownHeight, float y) 
        {
            var label = this.AddUIComponent<UILabel>();
            label.text = text;
            label.textScale = .85f;
            label.textColor = new Color32(200, 200, 200, 255);
            label.autoSize = false;
            label.width = width;
            label.textAlignment = UIHorizontalAlignment.Right;
            label.relativePosition = new Vector3(0, y + Mathf.Round((dropDownHeight - label.height) / 2));
        }

        public override void Populate(NetInfo prefab)
        {
            throw new System.NotImplementedException();
        }
    }
}
