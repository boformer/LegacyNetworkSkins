using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins
{
    public abstract class UINetworkOption : UIPanel
    {
        protected abstract void Initialize();
        
        public abstract void Populate(NetInfo prefab);

        protected float ParentWidth 
        {
            get 
            {
                return this.transform.parent.gameObject.GetComponent<UIComponent>().width - UINetworkSkinsPanel.PADDING * 2; 
            }
        }

        public override void Awake()
        {
            base.Awake();

            this.backgroundSprite = "SubcategoriesPanel";
            this.color = new Color32(0, 0, 255, 255);

            this.Initialize();

            this.FitChildrenVertically();
        }
    }
}
