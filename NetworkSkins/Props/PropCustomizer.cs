using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class PropCustomizer : ILoadingExtension
    {
        public static PropCustomizer instance;

        public PropCustomizerData data;
        public Dictionary<uint, SegmentPropDef> segmentPropMap = null;
        
        public void OnCreated(ILoading loading)
        {
            instance = this;

            return;

            NetInfoDetour.Deploy();
            NetLaneDetour.Deploy();
            NetManagerDetour.Deploy();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            return;
            
            var trees = new List<TreeInfo>();

            for (uint i = 0; i < PrefabCollection<TreeInfo>.LoadedCount(); i++) 
            {
                TreeInfo prefab = PrefabCollection<TreeInfo>.GetLoaded(i);

                if (prefab == null) continue;
                if (prefab.m_variations != null && prefab.m_variations.Length > 0) continue;

                trees.Add(prefab);
            }

            ADebugger.instance.trees = trees.ToArray();
            ADebugger.instance.b_propCustomizer = this;

            if(data == null) 
            {
                Debug.LogWarning("prop data is null!");

                data = new PropCustomizerData();
                data.SegmentPropDefs.Add(new SegmentPropDef { StreetLight = "Test Light", DisplayDecals = false });
                data.SegmentPropDefs.Add(new SegmentPropDef { Tree0 = "A Tree", Tree1 = "A different tree", DisplayDecals = true });
                data.SegmentPropDefs.Add(new SegmentPropDef { });
            }
        }

        public void OnLevelUnloading()
        {
            ADebugger.instance.trees = null;
            ADebugger.instance.b_propCustomizer = null;
        }

        public void OnReleased()
        {
            instance = null;
            
            NetInfoDetour.Revert();
            NetLaneDetour.Revert();
            NetManagerDetour.Revert();
        }
    }
}
