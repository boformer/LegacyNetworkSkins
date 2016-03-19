using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ICities;
using NetworkSkins.Data;
using NetworkSkins.Pillars;
using UnityEngine;

namespace NetworkSkins.Buildings
{
    // TODO refactor (2 files)
    public class BuildingToolWatcher : ThreadingExtensionBase
    {
        private const string NoneValue = "#None";

        internal static readonly Dictionary<BuildingInfo, BuildingNetworkSkinsDef.Building> _buildingDefsMap = new Dictionary<BuildingInfo, BuildingNetworkSkinsDef.Building>();

        private readonly Dictionary<NetInfo, BuildingInfo> _userModeBridgePillars = new Dictionary<NetInfo, BuildingInfo>();
        private readonly Dictionary<NetInfo, BuildingInfo> _userModeMiddlePillars = new Dictionary<NetInfo, BuildingInfo>();

        private BuildingInfo _selectedPrefab;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            var newSelectedPrefab = ToolsModifierControl.GetCurrentTool<BuildingTool>()?.m_prefab;

            if (_selectedPrefab == newSelectedPrefab) return;

            if (newSelectedPrefab != null)
            {
                SegmentDataManager.Instance.SetAssetMode(true);

                Revert();
                Apply(newSelectedPrefab);
            }
            else
            {
                SegmentDataManager.Instance.SetAssetMode(false);

                Revert();
            }

            _selectedPrefab = newSelectedPrefab;
        }

        private void Revert()
        {
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

            foreach (var networkDef in buildingDef.NetworkDefs)
            {
                var netInfo = PrefabCollection<NetInfo>.FindLoaded(networkDef.Name);
                if (netInfo == null) continue;

                var data = new SegmentData();

                if (networkDef.StreetLight != null)
                {
                    PropInfo streetLight = null;
                    if (networkDef.StreetLight != NoneValue)
                        streetLight = PrefabCollection<PropInfo>.FindLoaded(networkDef.StreetLight);
                    data.SetPrefabFeature(SegmentData.FeatureFlags.StreetLight, streetLight);
                }
                SetTreeFeature(data, networkDef.TreeLeft, SegmentData.FeatureFlags.TreeLeft);
                SetTreeFeature(data, networkDef.TreeMiddle, SegmentData.FeatureFlags.TreeMiddle);
                SetTreeFeature(data, networkDef.TreeRight, SegmentData.FeatureFlags.TreeRight);

                SetPillarFeature(netInfo, networkDef.BridgePillar, PillarType.BridgePillar, _userModeBridgePillars);
                SetPillarFeature(netInfo, networkDef.MiddlePillar, PillarType.MiddlePillar, _userModeMiddlePillars);

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
