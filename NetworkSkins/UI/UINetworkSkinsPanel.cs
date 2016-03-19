using System;
using System.IO;
using System.Linq;
using CimTools.Utilities;
using ColossalFramework.UI;
using NetworkSkins.Net;
using NetworkSkins.Pillars;
using NetworkSkins.Props;
using UnityEngine;

namespace NetworkSkins.UI
{
    public class UINetworkSkinsPanel : UIPanel
    {
        public const string Atlas = "NetworkSkinsSprites";
        
        public const int PaddingTop = 9;
        public const int Padding = 7;
        public const int PagesPadding = 10;
        public const int TabHeight = 32;
        public const int PageHeight = 140;//300;
        public const int Width = 360;//310;
        
        private UIDragHandle _titleBar;
        private UITabstrip _tabstrip;

        // The panels containing the net options (e.g. dropdowns)
        private UIPanel[] _netTypePages; 

        private INetToolWrapper _netToolWrapper;
        private NetInfo _selectedPrefab;
        private NetInfo[] _subPrefabs;

        public override void Awake()
        {
            base.Awake();

            Debug.Log("Begin UINetworkSkinsPanel.Awake");

            LoadSprites();

            this.backgroundSprite = "MenuPanel2";
            this.width = Width + 2 * Padding;
            //this.padding = new RectOffset(PADDING, 0, PADDING, 0);

            _titleBar = this.AddUIComponent<UIDragHandle>();
            _titleBar.name = "TitlePanel";
            _titleBar.width = this.width;
            _titleBar.height = TabHeight + PaddingTop;
            _titleBar.target = this;
            _titleBar.relativePosition = new Vector3(0, 0);

            // display a drag cursor sprite in the top right corner of the panel
            var dragSprite = _titleBar.AddUIComponent<UISprite>();
            dragSprite.atlas = SpriteUtilities.GetAtlas(Atlas);
            dragSprite.spriteName = "DragCursor";
            dragSprite.relativePosition = new Vector3(Width - 20, PaddingTop + 1);
            dragSprite.MakePixelPerfect();

            _tabstrip = _titleBar.AddUIComponent<UITabstrip>();
            _tabstrip.relativePosition = new Vector3(Padding, PaddingTop, 0);
            _tabstrip.width = Width;
            _tabstrip.height = TabHeight;
            _tabstrip.tabPages = this.AddUIComponent<UITabContainer>();
            _tabstrip.tabPages.width = this.width;
            _tabstrip.tabPages.height = PageHeight;
            _tabstrip.tabPages.relativePosition = new Vector3(0, _titleBar.height);
            _tabstrip.tabPages.padding = new RectOffset(PagesPadding, PagesPadding, PagesPadding, PagesPadding);
            _tabstrip.padding.right = 0;

            // Add 4 tabs and 4 pages
            var keyMappingTabstrip = GameObject.Find("KeyMappingTabStrip").GetComponent<UITabstrip>();
            var buttonTemplate = keyMappingTabstrip.GetComponentInChildren<UIButton>();

            _netTypePages = new UIPanel[NetUtil.NET_TYPE_NAMES.Length];

            for (var i = 0; i < NetUtil.NET_TYPE_NAMES.Length; i++)
            {
                var tab = _tabstrip.AddTab(NetUtil.NET_TYPE_NAMES[i], buttonTemplate, true);
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

                var page = _tabstrip.tabPages.components.Last() as UIPanel;
                page.autoLayoutDirection = LayoutDirection.Vertical;
                page.autoLayoutPadding = new RectOffset(0, 0, 0, Padding);
                page.autoLayout = true;
                page.isVisible = false;

                // TODO add scrolling + autofitting

                _netTypePages[i] = page;
            }
            
            this.FitChildren();


            _netToolWrapper = NetUtil.GenerateNetToolWrapper();
            if (_netToolWrapper == null) throw new Exception("NetworkSkins Error: NetToolWrapper is null!");

            // Add some example options
            GetPage(NetType.Ground).AddUIComponent<UILightOption>();
            //GetPage(NetType.Ground).AddUIComponent<UILightDistanceOption>();
            GetPage(NetType.Ground).AddUIComponent<UITreeOption>().LanePosition = LanePosition.Left;
            //GetPage(NetType.Ground).AddUIComponent<UITreeDistanceOption>().LanePosition = LanePosition.Left;
            GetPage(NetType.Ground).AddUIComponent<UITreeOption>().LanePosition = LanePosition.Middle;
            //GetPage(NetType.Ground).AddUIComponent<UITreeDistanceOption>().LanePosition = LanePosition.Middle;
            GetPage(NetType.Ground).AddUIComponent<UITreeOption>().LanePosition = LanePosition.Right;
            //GetPage(NetType.Ground).AddUIComponent<UITreeDistanceOption>().LanePosition = LanePosition.Right;

            GetPage(NetType.Elevated).AddUIComponent<UILightOption>();
            //GetPage(NetType.Elevated).AddUIComponent<UILightDistanceOption>();
            GetPage(NetType.Elevated).AddUIComponent<UIPillarOption>().PillarType = PillarType.BridgePillar;
            //GetPage(NetType.ELEVATED).AddUIComponent<UIPillarOption>().PillarType = PillarType.MIDDLE_PILLAR;

            GetPage(NetType.Bridge).AddUIComponent<UILightOption>();
            //GetPage(NetType.Bridge).AddUIComponent<UILightDistanceOption>();
            GetPage(NetType.Bridge).AddUIComponent<UIPillarOption>().PillarType = PillarType.BridgePillar;
            //GetPage(NetType.BRIDGE).AddUIComponent<UIPillarOption>().PillarType = PillarType.MIDDLE_PILLAR;
            //GetPage(NetType.BRIDGE).AddUIComponent<UIBridgeTypeOption>();

            _tabstrip.startSelectedIndex = (int)NetType.Ground;

            Debug.Log("End UINetworkSkinsPanel.Awake");
        }

        public override void Start()
        {
            absolutePosition = new Vector3(Mathf.Floor((GetUIView().GetScreenResolution().x - width - 50)), Mathf.Floor((GetUIView().GetScreenResolution().y - height - 50)));
        }

        private UIPanel GetPage(NetType netType) 
        {
            return _netTypePages[(int)netType];
        }

        /// <summary>
        /// Loads GUI sprites
        /// </summary>
        private void LoadSprites() 
        {
            try
            {
                if (SpriteUtilities.GetAtlas(Atlas) != null) return;

                var atlasSuccess = SpriteUtilities.InitialiseAtlas(Path.Combine(NetworkSkinsMod.GetModPath(), "sprites.png"), Atlas);

                if (atlasSuccess)
                {
                    var spriteSuccess = true;

                    spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(0, 0), new Vector2(24, 24)), "DragCursor", Atlas) && spriteSuccess;

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
            var newSelectedPrefab = _netToolWrapper.GetCurrentPrefab();

            if (newSelectedPrefab != null)
            {
                if (_selectedPrefab == newSelectedPrefab) return;
                _selectedPrefab = newSelectedPrefab;

                var newSubPrefabs = NetUtil.GetSubPrefabs(_selectedPrefab);

                if (_subPrefabs != null && _subPrefabs.SequenceEqual(newSubPrefabs)) return;
                _subPrefabs = newSubPrefabs;

                var visibleTabCount = 0;
                var firstVisibleIndex = -1;

                // Populate tabs and options
                for (var i = 0; i < _subPrefabs.Length; i++) 
                {
                    var tabName = NetUtil.NET_TYPE_NAMES[i];
                    
                    if (_subPrefabs[i] != null)
                    {
                        if (firstVisibleIndex < 0) firstVisibleIndex = i;
                        
                        var visibleOptionCount = 0;

                        foreach (var component in _netTypePages[i].components)
                        {
                            var option = component as UIOption;
                            if (option == null) continue;

                            // Pass the current prefab to the context-sensitive option
                            if (option.Populate(_subPrefabs[i])) visibleOptionCount++;
                        }

                        if (visibleOptionCount > 0)
                        {
                            visibleTabCount++;
                            _tabstrip.ShowTab(tabName);
                        }
                        else
                        {
                            _tabstrip.HideTab(tabName);
                        }
                    }
                    else
                    {
                        // Hide unrelevant tabs
                        _tabstrip.HideTab(tabName);
                    }
                }

                if (_subPrefabs[_tabstrip.selectedIndex] == null)
                {
                    if (_subPrefabs[(int)NetType.Ground] != null)
                    {
                        _tabstrip.selectedIndex = (int)NetType.Ground;
                    }
                    else if (firstVisibleIndex >= 0)
                    {
                        _tabstrip.selectedIndex = firstVisibleIndex;
                    }
                }
                else
                {
                    _tabstrip.selectedIndex = _tabstrip.selectedIndex;
                }

                isVisible = visibleTabCount > 0;
                return;
            }

            if (isVisible)
            {
                isVisible = false;
                _selectedPrefab = null;
                _subPrefabs = null;
            }
        }
    }
}
