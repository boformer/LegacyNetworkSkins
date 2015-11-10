
using ColossalFramework.UI;
using UnityEngine;
namespace NetworkSkins
{
    public abstract class UINetworkOption : UIPanel
    {
        public abstract void Populate(NetInfo prefab);

        protected float ParentWidth 
        {
            get 
            { 
                return this.transform.parent.gameObject.GetComponent<UIComponent>().width; 
            }
        }

        public override void Awake()
        {
            base.Awake();

            this.backgroundSprite = "SubcategoriesPanel";
            this.color = new Color32(0, 0, 255, 255);
        }
    }
}
