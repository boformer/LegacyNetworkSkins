using ColossalFramework.UI;
using NetworkSkins.Meshes;
using NetworkSkins.Pillars;
using NetworkSkins.Props;
using NetworkSkins.Net;
using System;
using System.Linq;
using UnityEngine;
using System.IO;
using CimTools.Utilities;

namespace NetworkSkins.UI
{
    public class UINetworkSkinsPanel : UIPanel
    {
        public const string ATLAS = "NetworkSkinsSprites";
        
        public const int PADDING_TOP = 9;
        public const int PADDING = 7;
        public const int PAGES_PADDING = 10;
        public const int TAB_HEIGHT = 32;
        public const int PAGE_HEIGHT = 300;
        public const int WIDTH = 310;
        
        private UIDragHandle titleBar;
        private UITabstrip tabstrip;

        // The panels containing the net options (e.g. dropdowns)
        private UIPanel[] netTypePages; 

        private INetToolWrapper netToolWrapper;
        private NetInfo selectedPrefab;
        private NetInfo[] subPrefabs;

        public override void Awake()
        {
            base.Awake();

            LoadSprites();

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

            // display a drag cursor sprite in the top right corner of the panel
            var dragSprite = titleBar.AddUIComponent<UISprite>();
            dragSprite.atlas = SpriteUtilities.GetAtlas(ATLAS);
            dragSprite.spriteName = "DragCursor";
            dragSprite.relativePosition = new Vector3(WIDTH - 20, PADDING_TOP + 1);
            dragSprite.MakePixelPerfect();

            tabstrip = titleBar.AddUIComponent<UITabstrip>();
            tabstrip.relativePosition = new Vector3(PADDING, PADDING_TOP, 0);
            tabstrip.width = WIDTH;
            tabstrip.height = TAB_HEIGHT;
            tabstrip.tabPages = this.AddUIComponent<UITabContainer>();
            tabstrip.tabPages.width = this.width;
            tabstrip.tabPages.height = PAGE_HEIGHT;
            tabstrip.tabPages.relativePosition = new Vector3(0, titleBar.height);
            tabstrip.tabPages.padding = new RectOffset(PAGES_PADDING, PAGES_PADDING, PAGES_PADDING, PAGES_PADDING);
            tabstrip.padding.right = 0;

            // Add 4 tabs and 4 pages
            UITabstrip keyMappingTabstrip = GameObject.Find("KeyMappingTabStrip").GetComponent<UITabstrip>();
            UIButton buttonTemplate = keyMappingTabstrip.GetComponentInChildren<UIButton>();

            netTypePages = new UIPanel[NetUtil.NET_TYPE_NAMES.Length];

            for (int i = 0; i < NetUtil.NET_TYPE_NAMES.Length; i++)
            {
                var tab = tabstrip.AddTab(NetUtil.NET_TYPE_NAMES[i], buttonTemplate, true);
                tab.textPadding.top = 8;
                tab.textPadding.bottom = 8;
                tab.textPadding.left = 10;
                tab.textPadding.right = 10;
                tab.autoSize = true;
                tab.textScale = .9f;
                tab.playAudioEvents = buttonTemplate.playAudioEvents;

                tab.pressedTextColor = new Color32(255, 255, 255, 255);
                tab.focusedTextColor = new Color32(255, 255, 255, 255);
                tab.focusedColor = new Color32(205, 205, 205, 255);
                tab.disabledTextColor = buttonTemplate.disabledTextColor;

                var page = tabstrip.tabPages.components.Last() as UIPanel;
                page.autoLayoutDirection = LayoutDirection.Vertical;
                page.autoLayoutPadding = new RectOffset(0, 0, 0, PADDING);
                page.autoLayout = true;
                page.isVisible = false;

                // TODO add scrolling + autofitting

                netTypePages[i] = page;
            }
            
            tabstrip.ShowTab("Ground");
            this.FitChildren();

            netToolWrapper = NetworkSkinsMod.GenerateNetToolWrapper();
            if (netToolWrapper == null) throw new Exception("NetworkSkins Error: NetToolWrapper is null!");

            // Add some example options
            GetPage(NetType.GROUND).AddUIComponent<UILightOption>();
            GetPage(NetType.GROUND).AddUIComponent<UITreeOption>();

            GetPage(NetType.ELEVATED).AddUIComponent<UILightOption>();
            GetPage(NetType.ELEVATED).AddUIComponent<UIPillarOption>();

            GetPage(NetType.BRIDGE).AddUIComponent<UILightOption>();
            GetPage(NetType.BRIDGE).AddUIComponent<UIPillarOption>();
            GetPage(NetType.BRIDGE).AddUIComponent<UIBridgeTypeOption>();
        }

        private UIPanel GetPage(NetType netType) 
        {
            return netTypePages[(int)netType];
        }

        /// <summary>
        /// Loads GUI sprites
        /// </summary>
        private void LoadSprites() 
        {
            try
            {
                bool atlasSuccess = SpriteUtilities.InitialiseAtlas(Path.Combine(NetworkSkinsMod.GetModPath(), "sprites.png"), ATLAS);

                if (atlasSuccess)
                {
                    bool spriteSuccess = true;

                    spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(24, 24)), "DragCursor", ATLAS) && spriteSuccess;

                    // TODO add sprites for tunnel, ground, elevated, bridge?

                    if (!spriteSuccess)
                    {
                        Debug.LogError("Network Skins: Failed to load some sprites!");
                    }
                }
                else
                {
                    Debug.LogError("Network Skins: Failed to load the atlas!");
                }
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Shows and populate the panel if NetTool is active, hide if not.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Fine Road Heights Net Tool support
            var newSelectedPrefab = netToolWrapper.GetCurrentPrefab();

            if (newSelectedPrefab != null)
            {
                if (selectedPrefab == newSelectedPrefab) return;
                selectedPrefab = newSelectedPrefab;

                var newSubPrefabs = NetUtil.GetSubPrefabs(selectedPrefab);

                if (subPrefabs != null && subPrefabs.SequenceEqual(newSubPrefabs)) return;
                subPrefabs = newSubPrefabs;

                int visibleTabCount = 0;
                int firstVisibleIndex = -1;

                // Populate tabs and options
                for (int i = 0; i < subPrefabs.Length; i++) 
                {
                    var tabName = NetUtil.NET_TYPE_NAMES[i];
                    
                    if (subPrefabs[i] != null)
                    {
                        visibleTabCount++;

                        if (firstVisibleIndex < 0) firstVisibleIndex = i;
                        
                        tabstrip.ShowTab(tabName);

                        foreach (UIComponent component in netTypePages[i].components)
                        {
                            var option = component as UINetworkOption;
                            if (option == null) continue;

                            // Pass the current prefab to the context-sensitive option
                            option.Populate(subPrefabs[i]);
                        }
                    }
                    else
                    {
                        // Hide unrelevant tabs
                        tabstrip.HideTab(tabName);
                    }
                }

                if (subPrefabs[tabstrip.selectedIndex] == null) 
                {
                    var groundIndex = (int)NetType.GROUND;
                    if (subPrefabs[groundIndex] != null)
                    {
                        tabstrip.selectedIndex = groundIndex;
                    }
                    else if (firstVisibleIndex >= 0)
                    {
                        tabstrip.selectedIndex = firstVisibleIndex;
                    }
                }

                isVisible = visibleTabCount > 0;
                return;
            }

            if (isVisible)
            {
                isVisible = false;
                selectedPrefab = null;
                subPrefabs = null;
            }
        }
    }
}
