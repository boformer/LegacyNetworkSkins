using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.UI
{
    public class UINetworkSkinsPanel : UIPanel
    {
        private const int PADDING = 5;
        
        private UIPanel titlePanel;
        private UIDragHandle dragHandle;

        private 

        public override void Awake()
        {
            base.Awake();

            this.backgroundSprite = "MenuPanel2";
            this.size = new Vector2(300, 300);
            this.relativePosition = new Vector3(0f, 440f);
            this.padding = new RectOffset(PADDING, 0, PADDING, 0);
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayout = true;

            titlePanel = this.AddUIComponent<UIPanel>();
            titlePanel.width = this.width;

            dragHandle = titlePanel.AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(30, 30);
            dragHandle.relativePosition = new Vector3(this.width - dragHandle.width, 0);

            var tabstrip = titlePanel.AddUIComponent<UITabstrip>();
            tabstrip.tabPages = this.AddUIComponent<UITabContainer>();
        }
    }
}
