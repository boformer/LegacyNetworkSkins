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

        private readonly List<SegmentPropDef> segmentPropDefs = new List<SegmentPropDef>();
        private readonly Dictionary<NetInfo, SegmentPropDef> activeSegmentDefs = new Dictionary<NetInfo, SegmentPropDef>();

        public SegmentPropDef[] segmentPropMap = null;

        // TODO replace with list, compile LaneProps
        public TreeInfo[] availableTrees = null;



        public void OnCreated(ILoading loading)
        {
            instance = this;

            NetInfoDetour.Deploy();
            NetLaneDetour.Deploy();
            NetManagerDetour.Deploy();
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            // Don't load if it's not a game
            if (!NetworkSkinsMod.CheckLoadMode(mode)) return;
            
            NetEventManager.instance.eventSegmentCreate += OnSegmentCreate;
            NetEventManager.instance.eventSegmentRelease += OnSegmentRelease;
            NetEventManager.instance.eventSegmentTransferData += OnSegmentTransferData;

            if (segmentPropMap == null)
            {
                segmentPropMap = new SegmentPropDef[NetManager.instance.m_segments.m_size];
            }

            var trees = new List<TreeInfo>();

            // no trees
            trees.Add(null);

            for (uint i = 0; i < PrefabCollection<TreeInfo>.LoadedCount(); i++) 
            {
                TreeInfo prefab = PrefabCollection<TreeInfo>.GetLoaded(i);

                if (prefab == null) continue;
                if (prefab.m_variations != null && prefab.m_variations.Length > 0) continue; // TODO remove this line?

                trees.Add(prefab);
            }

            // TODO search for prefabs for deserialized SegmentPropDefs

            availableTrees = trees.ToArray();

            // DEBUG access - remove later!
            ADebugger.instance.b_propCustomizer = this;
        }

        public void OnLevelUnloading()
        {
            NetEventManager.instance.eventSegmentCreate -= OnSegmentCreate;
            NetEventManager.instance.eventSegmentCreate -= OnSegmentRelease;
            NetEventManager.instance.eventSegmentTransferData -= OnSegmentTransferData;

            segmentPropDefs.Clear();

            if (segmentPropMap != null)
            {
                for (int i = 0; i < segmentPropMap.Length; i++) 
                {
                    segmentPropMap[i] = null;
                }
            }

            availableTrees = null;
            activeSegmentDefs.Clear();

            ADebugger.instance.b_propCustomizer = null;
        }

        public void OnReleased()
        {
            instance = null;
            
            NetInfoDetour.Revert();
            NetLaneDetour.Revert();
            NetManagerDetour.Revert();
        }

        public bool HasTrees(NetInfo prefab, LanePosition position) 
        {
            if(prefab.m_lanes != null)
                foreach (var lane in prefab.m_lanes)
                    if (lane != null && lane.m_laneProps != null && lane.m_laneProps.m_props != null && position.IsCorrectSide(lane.m_position))
                        foreach (var laneProp in lane.m_laneProps.m_props)
                            if (laneProp != null)
                                if (laneProp.m_finalTree != null) return true;
            return false;
        }

        public List<TreeInfo> GetAvailableTrees(NetInfo prefab)
        {
            return new List<TreeInfo>(availableTrees);
        }

        public TreeInfo GetActiveTree(NetInfo prefab, LanePosition position)
        {
            var def = new SegmentPropDef(GetSegmentPropDef(prefab));
            var flag = GetTreeFeatureFlag(position);

            if ((def.features & flag) == 0)
            {
                return GetDefaultTree(prefab, position);
            }
            else
            {
                switch (position)
                {
                    case LanePosition.LEFT: return def.treeLeftPrefab;
                    case LanePosition.MIDDLE: return def.treeMiddlePrefab;
                    case LanePosition.RIGHT: return def.treeRightPrefab;
                    default: throw new ArgumentOutOfRangeException("position");
                }
            }
        }

        public TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position)
        {

            if (prefab.m_lanes != null)
                foreach (var lane in prefab.m_lanes)
                    if (lane != null && lane.m_laneProps != null && lane.m_laneProps.m_props != null && position.IsCorrectSide(lane.m_position))
                        foreach (var laneProp in lane.m_laneProps.m_props)
                            if (laneProp != null)
                                if (laneProp.m_finalTree != null) return laneProp.m_finalTree;
            return null;
        }

        public void SetTree(NetInfo prefab, LanePosition position, TreeInfo tree)
        {
            var def = new SegmentPropDef(GetSegmentPropDef(prefab));
            var flag = GetTreeFeatureFlag(position);

            string treeName;

            if (tree == GetDefaultTree(prefab, position))
            {
                def.features = (def.features & ~flag);

                tree = null;
                treeName = null;
            }
            else
            {
                def.features = (def.features | flag);

                treeName = tree == null ? null : tree.name;
            }

            switch (position)
            {
                case LanePosition.LEFT: def.treeLeftPrefab = tree; def.treeLeft = treeName; break;
                case LanePosition.MIDDLE: def.treeMiddlePrefab = tree; def.treeMiddle = treeName; break;
                case LanePosition.RIGHT: def.treeRightPrefab = tree; def.treeRight = treeName; break;
                default: throw new ArgumentOutOfRangeException("position");
            }

            if (def.features == 0)
            {
                activeSegmentDefs.Remove(prefab);
                return;
            }
            else
            {
                var index = segmentPropDefs.IndexOf(def);
                if (index < 0)
                {
                    segmentPropDefs.Add(def);
                    activeSegmentDefs[prefab] = def;
                }
                else
                {
                    def = segmentPropDefs[index];
                    activeSegmentDefs[prefab] = def;
                }
            }
        }

        private SegmentPropDef GetSegmentPropDef(NetInfo prefab) 
        {
            SegmentPropDef def;
            activeSegmentDefs.TryGetValue(prefab, out def);
            return def;
        }

        private SegmentPropDef.Features GetTreeFeatureFlag(LanePosition position) 
        {
            switch (position)
            {
                case LanePosition.LEFT:   return SegmentPropDef.Features.TREE_LEFT;
                case LanePosition.MIDDLE: return SegmentPropDef.Features.TREE_MIDDLE;
                case LanePosition.RIGHT:  return SegmentPropDef.Features.TREE_RIGHT;
                default: throw new ArgumentOutOfRangeException("position");
            }
        }

        public void OnSegmentCreate(ushort segment)
        {
            var prefab = NetManager.instance.m_segments.m_buffer[segment].Info;
            var def = GetSegmentPropDef(prefab);
            segmentPropMap[segment] = def;
            
            Debug.LogFormat("Segment {0} created!", segment);

            if (def == null) Debug.Log("Def is null!");
            else Debug.Log(prefab.name + ": " + def);
        }

        public void OnSegmentRelease(ushort segment)
        {
            segmentPropMap[segment] = null;
            
            Debug.LogFormat("Segment {0} released!", segment);
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            segmentPropMap[newSegment] = segmentPropMap[oldSegment];

            Debug.LogFormat("Transfer data from {0} to {1}!", oldSegment, newSegment);
        }
    }
}
