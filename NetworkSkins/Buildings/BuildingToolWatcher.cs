using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ICities;
using NetworkSkins.Data;
using NetworkSkins.Pillars;
using UnityEngine;

namespace NetworkSkins.Buildings
{
    /// <summary>
    /// You can bundle a BuildingNetworkSkinsDef.xml file with a building to modify its networks.
    /// That means when the building is placed, NetworkSkins applies the configured trees,
    /// street lights and bridge pillars to the internal networks of the building.
    /// 
    /// To do that, a special "building mode" is enabled while the building tool is active.
    /// Previous user settings are restored when the building tool is no longer in use.
    /// </summary>
    public class BuildingToolWatcher : ThreadingExtensionBase
    {
        private const string NoneValue = "#None";

        internal static readonly Dictionary<BuildingInfo, BuildingNetworkSkinsDef.Building> _buildingDefsMap = new Dictionary<BuildingInfo, BuildingNetworkSkinsDef.Building>();

        // Stores which pillar was selected for a particular network before the building was added
        private readonly Dictionary<NetInfo, BuildingInfo> _userModeBridgePillars = new Dictionary<NetInfo, BuildingInfo>();
        private readonly Dictionary<NetInfo, BuildingInfo> _userModeMiddlePillars = new Dictionary<NetInfo, BuildingInfo>();

        private BuildingInfo _selectedPrefab;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            var newSelectedPrefab = ToolsModifierControl.GetCurrentTool<BuildingTool>()?.m_prefab;

            if (_selectedPrefab == newSelectedPrefab) return;

            // If the user selected a building for placement, apply the configuration for the building
            if (newSelectedPrefab != null)
            {
                // Instruct SDM to store settings in a separate map, so when asset mode is set to false again,
                // NetworkSkins is able to reload user-selected settings
                SegmentDataManager.Instance.SetAssetMode(true);

                Revert();
                Apply(newSelectedPrefab);
            }

            // when the building tool is no longer in use, return to mode for normal networks
            else
            {
                // Clear asset mode settings, return to user-selected settings
                SegmentDataManager.Instance.SetAssetMode(false);

                Revert();
            }

            _selectedPrefab = newSelectedPrefab;
        }

        private void Revert()
        {
            // restore bridge pillars for user-placed networks
            foreach (var entry in _userModeBridgePillars)
            {
                PillarCustomizer.instance.SetPillar(entry.Key, PillarType.BridgePillar, entry.Value);
            }
            foreach (var entry in _userModeMiddlePillars)
            {
                PillarCustomizer.instance.SetPillar(entry.Key, PillarType.MiddlePillar, entry.Value);
            }
            _userModeBridgePillars.Clear();
            _userModeMiddlePillars.Clear();
        }

        private void Apply(BuildingInfo prefab)
        {
            BuildingNetworkSkinsDef.Building buildingDef;
            if (!_buildingDefsMap.TryGetValue(prefab, out buildingDef)) return;

            // Apply config for networks which are placed with a building
            foreach (var networkDef in buildingDef.NetworkDefs)
            {
                var netInfo = PrefabCollection<NetInfo>.FindLoaded(networkDef.Name);
                if (netInfo == null) continue;

                var data = new SegmentData();

                // Street lights
                if (networkDef.StreetLight != null)
                {
                    PropInfo streetLight = null;
                    if (networkDef.StreetLight != NoneValue)
                        streetLight = PrefabCollection<PropInfo>.FindLoaded(networkDef.StreetLight);
                    data.SetPrefabFeature(SegmentData.FeatureFlags.StreetLight, streetLight);
                }

                // Trees
                SetTreeFeature(data, networkDef.TreeLeft, SegmentData.FeatureFlags.TreeLeft);
                SetTreeFeature(data, networkDef.TreeMiddle, SegmentData.FeatureFlags.TreeMiddle);
                SetTreeFeature(data, networkDef.TreeRight, SegmentData.FeatureFlags.TreeRight);

                // Pillars
                SetPillarFeature(netInfo, networkDef.BridgePillar, PillarType.BridgePillar, _userModeBridgePillars);
                SetPillarFeature(netInfo, networkDef.MiddlePillar, PillarType.MiddlePillar, _userModeMiddlePillars);

                // Apply options for asset mode (SetAssetMode was set to true before,
                // so the SegmentDataManager stores these options in a separate map
                // that is cleared when asset mode is left)
                SegmentDataManager.Instance.SetActiveOptions(netInfo, data);
            }
        }

        private static void SetTreeFeature(SegmentData data, string treeName, SegmentData.FeatureFlags flag)
        {
            if (treeName == null) return;

            TreeInfo tree = null;
            if (treeName != NoneValue) tree = PrefabCollection<TreeInfo>.FindLoaded(treeName);

            data.SetPrefabFeature(flag, tree);
        }

        private static void SetPillarFeature(NetInfo netInfo, string pillarName, PillarType type,
            Dictionary<NetInfo, BuildingInfo> userModePillars)
        {
            if (pillarName == null) return;

            BuildingInfo pillar = null;
            if (pillarName != NoneValue) pillar = PrefabCollection<BuildingInfo>.FindLoaded(pillarName);

            // save previous set pillar
            userModePillars[netInfo] = PillarCustomizer.instance.GetActivePillar(netInfo, type);

            PillarCustomizer.instance.SetPillar(netInfo, type, pillar);
        }
    }

    /// <summary>
    /// Load building network configurations into memory
    /// </summary>
    public class BuildingDefLoader : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            var parsedFiles = new HashSet<string>();

            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetLoaded(i);

                if (prefab == null) continue;

                var asset = PackageManager.FindAssetByName(prefab.name);

                var crpPath = asset?.package?.packagePath;
                if (crpPath == null) continue;

                var defConfigPath = Path.Combine(Path.GetDirectoryName(crpPath), "BuildingNetworkSkinsDef.xml");

                if (parsedFiles.Contains(defConfigPath)) continue;

                parsedFiles.Add(defConfigPath);

                if (!File.Exists(defConfigPath)) continue;

                BuildingNetworkSkinsDef def = null;

                var xmlSerializer = new XmlSerializer(typeof(BuildingNetworkSkinsDef));
                try
                {
                    using (var streamReader = new System.IO.StreamReader(defConfigPath))
                    {
                        def = (BuildingNetworkSkinsDef)xmlSerializer.Deserialize(streamReader);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (def == null) return;

                foreach (var buildingDef in def.Buildings)
                {
                    var buildingInfo = FindPrefab(buildingDef.Name, asset.package.packageName);

                    if (buildingInfo == null || buildingDef.NetworkDefs == null || buildingDef.NetworkDefs.Count == 0) continue;

                    BuildingToolWatcher._buildingDefsMap.Add(buildingInfo, buildingDef);
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            BuildingToolWatcher._buildingDefsMap.Clear();
        }

        private static BuildingInfo FindPrefab(string prefabName, string packageName)
        {
            var prefab = PrefabCollection<BuildingInfo>.FindLoaded(prefabName);
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(prefabName + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(PathEscaper.Escape(prefabName) + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(packageName + "." + prefabName + "_Data");
            if (prefab == null) prefab = PrefabCollection<BuildingInfo>.FindLoaded(packageName + "." + PathEscaper.Escape(prefabName) + "_Data");

            return prefab;
        }
    }
}
