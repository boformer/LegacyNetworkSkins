using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.Net;
using System;
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

        // TODO replace with list, compile LaneProps
        public TreeInfo[] networkTrees = null;

        private readonly Dictionary<NetInfo, TreeInfo> activeLeftTrees = new Dictionary<NetInfo, TreeInfo>();
        private readonly Dictionary<NetInfo, TreeInfo> activeMiddleTrees = new Dictionary<NetInfo, TreeInfo>();
        private readonly Dictionary<NetInfo, TreeInfo> activeRightTrees = new Dictionary<NetInfo, TreeInfo>();

        public void OnCreated(ILoading loading)
        {
            instance = this;

            NetInfoDetour.Deploy();
            NetLaneDetour.Deploy();
            NetManagerDetour.Deploy();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            NetEventManager.instance.eventSegmentCreate += OnSegmentCreate;
            NetEventManager.instance.eventSegmentRelease += OnSegmentRelease;
            NetEventManager.instance.eventSegmentTransferData += OnSegmentTransferData;

            var trees = new List<TreeInfo>();

            for (uint i = 0; i < PrefabCollection<TreeInfo>.LoadedCount(); i++) 
            {
                TreeInfo prefab = PrefabCollection<TreeInfo>.GetLoaded(i);

                if (prefab == null) continue;
                if (prefab.m_variations != null && prefab.m_variations.Length > 0) continue;

                trees.Add(prefab);
            }

            networkTrees = trees.ToArray();

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
            NetEventManager.instance.eventSegmentCreate -= OnSegmentCreate;
            NetEventManager.instance.eventSegmentCreate -= OnSegmentRelease;
            NetEventManager.instance.eventSegmentTransferData -= OnSegmentTransferData;

            networkTrees = null;
            activeLeftTrees.Clear();
            activeMiddleTrees.Clear();
            activeRightTrees.Clear();

            ADebugger.instance.b_propCustomizer = null;
        }

        public void OnReleased()
        {
            instance = null;
            
            NetInfoDetour.Revert();
            NetLaneDetour.Revert();
            NetManagerDetour.Revert();
        }

        public bool HasTrees(NetInfo prefab, LanePosition type) 
        {
            if(prefab.m_lanes != null)
                foreach (var lane in prefab.m_lanes)
                    if (lane != null && lane.m_laneProps != null && lane.m_laneProps.m_props != null && IsCorrectSide(lane.m_position, type))
                        foreach (var laneProp in lane.m_laneProps.m_props)
                            if (laneProp != null)
                                if (laneProp.m_tree != null) return true;
            return false;
        }

        public List<TreeInfo> GetAvailableTrees(NetInfo prefab)
        {
            return new List<TreeInfo>(networkTrees);
        }

        public TreeInfo GetActiveTree(NetInfo prefab, LanePosition type)
        {
            var map = GetActiveTreeMap(type);

            TreeInfo activeTree;
            if (!map.TryGetValue(prefab, out activeTree)) return GetDefaultTree(prefab, type);

            return activeTree;
        }

        public TreeInfo GetDefaultTree(NetInfo prefab, LanePosition type)
        {

            if (prefab.m_lanes != null)
                foreach (var lane in prefab.m_lanes)
                    if (lane != null && lane.m_laneProps != null && lane.m_laneProps.m_props != null && IsCorrectSide(lane.m_position, type))
                        foreach (var laneProp in lane.m_laneProps.m_props)
                            if (laneProp != null)
                                if (laneProp.m_tree != null) return laneProp.m_tree;
            return null;
        }

        public void SetTree(NetInfo prefab, LanePosition type, TreeInfo tree)
        {
            var map = GetActiveTreeMap(type);

            if (tree == GetDefaultTree(prefab, type))
                map.Remove(prefab);
            else
                map[prefab] = tree;
        }

        private Dictionary<NetInfo, TreeInfo> GetActiveTreeMap(LanePosition type) 
        {
            switch (type) 
            {
                case LanePosition.LEFT:
                    return activeLeftTrees;
                case LanePosition.MIDDLE:
                    return activeMiddleTrees;
                case LanePosition.RIGHT:
                    return activeRightTrees;
                default:
                    throw new Exception("Network Skins: Unknown Tree Type!");
            }
        }

        private bool IsCorrectSide(float lanePosition, LanePosition type)
        {
            if (type == LanePosition.MIDDLE) 
                return lanePosition == 0f;
            else
                return (type == LanePosition.LEFT) == (lanePosition < 0);
        }

        public void OnSegmentCreate(ushort segment)
        {
            Debug.LogFormat("Segment {0} created!", segment);
        }

        public void OnSegmentRelease(ushort segment)
        {
            Debug.LogFormat("Segment {0} released!", segment);
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            Debug.LogFormat("Transfer data from {0} to {1}!", oldSegment, newSegment);
        }
    }
}
