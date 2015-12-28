using ColossalFramework.UI;
using NetworkSkins.UI;
using UnityEngine;

namespace NetworkSkins
{
    public abstract class UINetworkOption : UIPanel
    {
        protected NetInfo SelectedPrefab { get; private set; }
        protected bool Populating { get; private set; }
        
        protected abstract void Initialize();

        public void Populate(NetInfo prefab) 
        {
            Populating = true;
            SelectedPrefab = prefab;
            PopulateImpl();
            Populating = false;
        }
        
        protected abstract void PopulateImpl();

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
