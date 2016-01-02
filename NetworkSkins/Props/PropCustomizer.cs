using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ICities;
using NetworkSkins.Data;
using NetworkSkins.Detour;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class PropCustomizer : LoadingExtensionBase
    {
        public static PropCustomizer Instance;

        private readonly List<TreeInfo> _availableTrees = new List<TreeInfo>();
        private readonly List<PropInfo> _availableStreetLights = new List<PropInfo>();
        public int[] StreetLightPrefabDataIndices;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            Instance = this;

            RenderManagerDetour.EventUpdateData += OnUpdateData;

            NetInfoDetour.Deploy();
            NetLaneDetour.Deploy();
            NetManagerDetour.Deploy();
        }

        /// <summary>
        /// Like OnLevelLoaded, but executed earlier.
        /// </summary>
        /// <param name="mode"></param>
        public void OnUpdateData(SimulationManager.UpdateMode mode)
        {
            if (mode != SimulationManager.UpdateMode.LoadGame && mode != SimulationManager.UpdateMode.NewGame) return;

            // no trees
            _availableTrees.Add(null);

            for (uint i = 0; i < PrefabCollection<TreeInfo>.LoadedCount(); i++) 
            {
                var prefab = PrefabCollection<TreeInfo>.GetLoaded(i);

                if (prefab == null) continue;

                _availableTrees.Add(prefab);
            }

            // no street lights
            _availableStreetLights.Add(null);

            for (uint i = 0; i < PrefabCollection<PropInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(i);

                if (prefab == null) continue;
                if (prefab.m_class.m_service == ItemClass.Service.Road || 
                    prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane || 
                    prefab.name.Contains("StreetLamp"))
                {
                    if (prefab.m_effects != null && prefab.m_effects.Length > 0) 
                    {
                        if (prefab.name.Contains("Taxiway")) continue;
                        if (prefab.name.Contains("Runway")) continue;
                        
                        if (prefab.m_effects.Where(effect => effect.m_effect != null).Any(effect => effect.m_effect is LightEffect))
                        {
                            _availableStreetLights.Add(prefab);
                        }
                    }
                }
            }

            // compile list of data indices for fast check if a prefab is a street light:
            StreetLightPrefabDataIndices = _availableStreetLights.Where(prop => prop != null).Select(prop => prop.m_prefabDataIndex).ToArray();
        }

        public override void OnLevelUnloading()
        {
            _availableTrees.Clear();
            _availableStreetLights.Clear();
        }

        public override void OnReleased()
        {
            Instance = null;

            RenderManagerDetour.EventUpdateData -= OnUpdateData;

            NetInfoDetour.Revert();
            NetLaneDetour.Revert();
            NetManagerDetour.Revert();
        }

        public bool HasTrees(NetInfo prefab, LanePosition position) 
        {
            if (prefab.m_lanes == null) return false;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalTree != null && _availableStreetLights.Contains(laneProp.m_finalProp)) return true;
                    }
            return false;
        }

        public bool HasStreetLights(NetInfo prefab)
        {
            if (prefab.m_lanes == null) return false;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalProp != null && _availableStreetLights.Contains(laneProp.m_finalProp)) return true;
                    }

            return false;
        }

        public List<TreeInfo> GetAvailableTrees(NetInfo prefab)
        {
            return _availableTrees;
        }

        public List<PropInfo> GetAvailableStreetLights(NetInfo prefab)
        {
            return _availableStreetLights;
        }

        public TreeInfo GetActiveTree(NetInfo prefab, LanePosition position)
        {
            var segmentData = SegmentDataManager.Instance.GetActiveSegmentData(prefab);

            if (segmentData == null || !segmentData.Features.IsFlagSet(position.ToTreeFeatureFlag()))
            {
                return GetDefaultTree(prefab, position);
            }
            else
            {
                switch (position)
                {
                    case LanePosition.Left: return segmentData.TreeLeftPrefab;
                    case LanePosition.Middle: return segmentData.TreeMiddlePrefab;
                    case LanePosition.Right: return segmentData.TreeRightPrefab;
                    default: throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }

        public PropInfo GetActiveStreetLight(NetInfo prefab)
        {
            var segmentData = SegmentDataManager.Instance.GetActiveSegmentData(prefab);

            if (segmentData == null || !segmentData.Features.IsFlagSet(SegmentData.FeatureFlags.StreetLight))
            {
                return GetDefaultStreetLight(prefab);
            }
            else
            {
                return segmentData.StreetLightPrefab;
            }
        }

        public TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position)
        {
            if (prefab.m_lanes == null) return null;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalTree != null) return laneProp.m_finalTree;
                    }

            return null;
        }

        public PropInfo GetDefaultStreetLight(NetInfo prefab)
        {
            if (prefab.m_lanes == null) return null;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalProp != null && _availableStreetLights.Contains(laneProp.m_finalProp)) return laneProp.m_finalProp;
                    }

            return null;
        }

        public void SetTree(NetInfo prefab, LanePosition position, TreeInfo tree)
        {
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveSegmentData(prefab));

            if (tree != GetDefaultTree(prefab, position))
            {
                newSegmentData.SetFeature(position.ToTreeFeatureFlag(), tree);
            }
            else
            {
                newSegmentData.UnsetFeature(position.ToTreeFeatureFlag());
            }

            SegmentDataManager.Instance.SetActiveSegmentData(prefab, newSegmentData);
        }

        public void SetStreetLight(NetInfo prefab, PropInfo prop)
        {
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveSegmentData(prefab));

            if (prop != GetDefaultStreetLight(prefab))
            {
                newSegmentData.SetFeature(SegmentData.FeatureFlags.StreetLight, prop);
            }
            else
            {
                newSegmentData.UnsetFeature(SegmentData.FeatureFlags.StreetLight);
            }

            SegmentDataManager.Instance.SetActiveSegmentData(prefab, newSegmentData);
        }
    }
}
