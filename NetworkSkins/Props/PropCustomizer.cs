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

        private List<TreeInfo> _availableTrees = new List<TreeInfo>();
        private List<PropInfo> _availableStreetLights = new List<PropInfo>();

        public int[] StreetLightPrefabDataIndices;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            Instance = this;

            RenderManagerDetour.EventUpdateData += OnUpdateData;

            NetLaneDetour.Deploy();
        }

        /// <summary>
        /// Like OnLevelLoaded, but executed earlier.
        /// </summary>
        /// <param name="mode"></param>
        public void OnUpdateData(SimulationManager.UpdateMode mode)
        {
            if (mode != SimulationManager.UpdateMode.LoadMap && mode != SimulationManager.UpdateMode.NewMap 
                && mode != SimulationManager.UpdateMode.LoadGame && mode != SimulationManager.UpdateMode.NewGameFromMap
                && mode != SimulationManager.UpdateMode.NewGameFromScenario) return;

            for (uint i = 0; i < PrefabCollection<TreeInfo>.LoadedCount(); i++) 
            {
                var prefab = PrefabCollection<TreeInfo>.GetLoaded(i);

                if (prefab == null) continue;

                _availableTrees.Add(prefab);
            }

	        _availableTrees = _availableTrees.OrderBy(prefab => UITreeOption.GenerateBeautifiedPrefabName(prefab).ToLowerInvariant().Replace("the ", "")).ToList();
			_availableTrees.Insert(0, null);

            for (uint i = 0; i < PrefabCollection<PropInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(i);

                if (prefab == null) continue;
                if (prefab.m_class.m_service == ItemClass.Service.Road || 
                    prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane || 
                    prefab.name.ToLower().Contains("streetlamp") || prefab.name.ToLower().Contains("streetlight") || prefab.name.ToLower().Contains("lantern"))
                {
                    if (prefab.m_effects != null && prefab.m_effects.Length > 0) 
                    {
                        if (prefab.name.ToLower().Contains("taxiway")) continue;
                        if (prefab.name.ToLower().Contains("runway")) continue;
                        
                        if (prefab.m_effects.Where(effect => effect.m_effect != null).Any(effect => effect.m_effect is LightEffect))
                        {
                            _availableStreetLights.Add(prefab);
                        }
                    }
                }
            }

	        _availableStreetLights = _availableStreetLights.OrderBy(prefab => prefab?.GetUncheckedLocalizedTitle().Trim().ToLowerInvariant().Replace("the ", "")).ToList();
			_availableStreetLights.Insert(0, null);

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

            NetLaneDetour.Revert();
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
            var segmentData = SegmentDataManager.Instance.GetActiveOptions(prefab);

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
            var segmentData = SegmentDataManager.Instance.GetActiveOptions(prefab);

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
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveOptions(prefab));

            if (tree != GetDefaultTree(prefab, position))
            {
                newSegmentData.SetPrefabFeature(position.ToTreeFeatureFlag(), tree);
            }
            else
            {
                newSegmentData.UnsetFeature(position.ToTreeFeatureFlag());
            }

            SegmentDataManager.Instance.SetActiveOptions(prefab, newSegmentData);
        }

        public void SetStreetLight(NetInfo prefab, PropInfo prop)
        {
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveOptions(prefab));

            if (prop != GetDefaultStreetLight(prefab))
            {
                newSegmentData.SetPrefabFeature(SegmentData.FeatureFlags.StreetLight, prop);
            }
            else
            {
                newSegmentData.UnsetFeature(SegmentData.FeatureFlags.StreetLight);
            }

            SegmentDataManager.Instance.SetActiveOptions(prefab, newSegmentData);
        }

        public void SetStreetLightDistance(NetInfo prefab, float val)
        {
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveOptions(prefab));

            var distanceVector = newSegmentData.RepeatDistances;
            distanceVector.w = Math.Abs(val - GetDefaultStreetLightDistance(prefab)) > .01f ? val : 0f;

            if (distanceVector != Vector4.zero)
            {
                newSegmentData.SetStructFeature(SegmentData.FeatureFlags.RepeatDistances, distanceVector);
            }
            else
            {
                newSegmentData.UnsetFeature(SegmentData.FeatureFlags.RepeatDistances);
            }

            SegmentDataManager.Instance.SetActiveOptions(prefab, newSegmentData);
        }

        public void SetTreeDistance(NetInfo prefab, LanePosition position, float val)
        {
            var newSegmentData = new SegmentData(SegmentDataManager.Instance.GetActiveOptions(prefab));

            var distanceVector = newSegmentData.RepeatDistances;
            var value = Mathf.Abs(val - GetDefaultTreeDistance(prefab, position)) > .01f ? val : 0f;

            switch (position)
            {
                case LanePosition.Left:
                    distanceVector.x = value;
                    break;
                case LanePosition.Middle:
                    distanceVector.y = value;
                    break;
                case LanePosition.Right:
                    distanceVector.z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (distanceVector != Vector4.zero)
            {
                newSegmentData.SetStructFeature(SegmentData.FeatureFlags.RepeatDistances, distanceVector);
            }
            else
            {
                newSegmentData.UnsetFeature(SegmentData.FeatureFlags.RepeatDistances);
            }

            SegmentDataManager.Instance.SetActiveOptions(prefab, newSegmentData);
        }

        public float GetDefaultStreetLightDistance(NetInfo prefab)
        {
            if (prefab.m_lanes == null) return -1f;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalProp != null && _availableStreetLights.Contains(laneProp.m_finalProp)) return laneProp.m_repeatDistance;
                    }

            return -1f;
        }

        public float GetDefaultTreeDistance(NetInfo prefab, LanePosition position)
        {
            if (prefab.m_lanes == null) return -1f;

            foreach (var lane in prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props)
                    {
                        if (laneProp?.m_finalTree != null) return laneProp.m_repeatDistance;
                    }

            return -1f;
        }

        public float GetActiveStreetLightDistance(NetInfo prefab)
        {
            var segmentData = SegmentDataManager.Instance.GetActiveOptions(prefab);

            if (segmentData != null && segmentData.Features.IsFlagSet(SegmentData.FeatureFlags.RepeatDistances) && segmentData.RepeatDistances.w > 0f)
            {
                return segmentData.RepeatDistances.w;
            }
            else
            {
                return GetDefaultStreetLightDistance(prefab);
            }
        }


        public float GetActiveTreeDistance(NetInfo prefab, LanePosition position)
        {
            var segmentData = SegmentDataManager.Instance.GetActiveOptions(prefab);

            var result = 0f;
            if (segmentData != null && segmentData.Features.IsFlagSet(SegmentData.FeatureFlags.RepeatDistances))
            {
                switch (position)
                {
                    case LanePosition.Left:
                        result = segmentData.RepeatDistances.x;
                        break;
                    case LanePosition.Middle:
                        result = segmentData.RepeatDistances.y;
                        break;
                    case LanePosition.Right:
                        result = segmentData.RepeatDistances.z;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position));
                }
            }

            return result > 0f ? result : GetDefaultTreeDistance(prefab, position);
        }
    }
}
