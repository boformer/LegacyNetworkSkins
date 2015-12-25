using ColossalFramework.UI;
using NetworkSkins.Pillars;
using UnityEngine;

namespace NetworkSkins.UI
{
    public class UINetworkSkinsPanel : UIPanel
    {
        public const int PADDING_TOP = 9;
        public const int PADDING = 5;
        public const int PAGES_PADDING = 10;
        public const int TAB_HEIGHT = 32;
        public const int PAGE_HEIGHT = 300;
        public const int WIDTH = 310;
        
        private UIDragHandle titleBar;

        private UITabstrip tabstrip;

        private UIPanel tunnelPage;
        private UIPanel groundPage;
        private UIPanel elevatedPage;
        private UIPanel bridgePage;

        public override void Awake()
        {
            base.Awake();

            this.backgroundSprite = "MenuPanel2";
            this.relativePosition = new Vector3(0f, 440f);
            this.width = WIDTH + 2 * PADDING;
            //this.padding = new RectOffset(PADDING, 0, PADDING, 0);

            titleBar = this.AddUIComponent<UIDragHandle>();
            titleBar.name = "TitlePanel";
            titleBar.width = this.width;
            titleBar.height = TAB_HEIGHT + PADDING_TOP;
            titleBar.target = this;
            titleBar.relativePosition = new Vector3(0, 0);

            tabstrip = titleBar.AddUIComponent<UITabstrip>();
            tabstrip.relativePosition = new Vector3(PADDING, PADDING_TOP, 0);
            tabstrip.width = WIDTH;
            tabstrip.height = TAB_HEIGHT;
            tabstrip.tabPages = this.AddUIComponent<UITabContainer>();
            tabstrip.tabPages.width = this.width;
            tabstrip.tabPages.height = PAGE_HEIGHT;
            tabstrip.tabPages.relativePosition = new Vector3(0, titleBar.height);
            tabstrip.tabPages.padding = new RectOffset(PAGES_PADDING, PAGES_PADDING, PAGES_PADDING, PAGES_PADDING);//?
            tabstrip.padding.right = 0;
            tabstrip.eventTabIndexChanged += tabstrip_eventTabIndexChanged;

            UITabstrip keyMappingTabstrip = GameObject.Find("KeyMappingTabStrip").GetComponent<UITabstrip>();
            UIButton buttonTemplate = keyMappingTabstrip.GetComponentInChildren<UIButton>();

            tabstrip.AddTab("Tunnel", buttonTemplate, true);   // 0
            tabstrip.AddTab("Ground", buttonTemplate, true);   // 1
            tabstrip.AddTab("Elevated", buttonTemplate, true); // 2
            tabstrip.AddTab("Bridge", buttonTemplate, true);   // 3

            foreach (var component in tabstrip.components) 
            {
                var tab = component as UIButton;

                tab.textPadding.top = 7;
                tab.textPadding.bottom = 7;
                tab.textPadding.left = 9;
                tab.textPadding.right = 9;
                tab.autoSize = true;
                //tab.textScale = .75f;

                tab.playAudioEvents = buttonTemplate.playAudioEvents;
                tab.pressedTextColor = new Color32(255, 255, 255, 255);
                tab.focusedTextColor = new Color32(255, 255, 255, 255);
                tab.disabledTextColor = buttonTemplate.disabledTextColor;
            }

            var i = 0;
            foreach (var component in tabstrip.tabPages.components) 
            {
                i++;
                var panel = component as UIPanel;
                
                //panel.backgroundSprite = "SubcategoriesPanel";
                panel.autoLayoutDirection = LayoutDirection.Vertical;
                panel.autoLayoutPadding = new RectOffset(0, 0, 0, PADDING);
                panel.autoLayout = true;
                panel.isVisible = false;
                //panel.size = new Vector2(WIDTH, 50 * i);
                //panel.color = new Color32(0, 0, (byte) (70 * i), 255);
            }

            tunnelPage = tabstrip.tabPages.components[0] as UIPanel;
            groundPage = tabstrip.tabPages.components[1] as UIPanel;
            elevatedPage = tabstrip.tabPages.components[2] as UIPanel;
            bridgePage = tabstrip.tabPages.components[3] as UIPanel;

            elevatedPage.AddUIComponent<PillarNetworkOption>();
            bridgePage.AddUIComponent<PillarNetworkOption>();

            tabstrip.ShowTab("Ground");
            this.FitChildren();
        }

        void tabstrip_eventTabIndexChanged(UIComponent component, int value)
        {
            //tabstrip.tabPages.FitChildren();
            //this.FitChildren(Vector2.one * PADDING);
        }
    }
}
