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
                return this.transform.parent.gameObject.GetComponent<UIComponent>().width; 
            }
        }

        public override void Awake()
        {
            base.Awake();
            this.Initialize();

            this.width = ParentWidth;
            this.FitChildrenVertically();
        }
    }
}
