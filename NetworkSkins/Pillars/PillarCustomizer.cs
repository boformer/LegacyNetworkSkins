﻿using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NetworkSkins.Pillars
{
    public class PillarCustomizer : ILoadingExtension
    {
        public static PillarCustomizer instance;
        
        private readonly List<BuildingInfo> pillarBuildings = new List<BuildingInfo>();

        private readonly Dictionary<NetInfo, BuildingInfo> defaultBridgePillars = new Dictionary<NetInfo, BuildingInfo>();
        private readonly Dictionary<NetInfo, BuildingInfo> defaultMiddlePillars = new Dictionary<NetInfo, BuildingInfo>();

        private PillarCustomizerPanel uiPanel;

        public void OnCreated(ILoading loading)
        {
            instance = this;
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            // Don't load if it's not a game
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame) return;

            // Save defaults
            for(uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<NetInfo>.GetLoaded(i);

                if (prefab == null) continue;

                SaveDefaults(prefab);
            }

            // support for custom pillars
            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetLoaded(i);

                if (prefab == null) continue;

                // only accept buildings with a basic AI
                if (prefab.m_buildingAI.GetType() != typeof(BuildingAI)) continue;

                var asset = PackageManager.FindAssetByName(prefab.name);
                if (asset == null || asset.package == null) continue;

                string crpPath = asset.package.packagePath;
                if (crpPath == null) continue;

                string pillarConfigPath = Path.Combine(Path.GetDirectoryName(crpPath), "Pillar.xml");

                // TODO parse the config file

                if (File.Exists(pillarConfigPath)) 
                {
                    pillarBuildings.Add(prefab);
                }
            }

            // TODO Detect No Pillars and hide its "pillars" button
            // TODO Add bridge mode support

            // Initialize GUI

            if (uiPanel == null)
            {
                if (FineRoadHeightsEnabled())
                {
                    uiPanel = UIView.GetAView().AddUIComponent(typeof(PillarCustomizerPanelFineRoadHeights)) as PillarCustomizerPanel;
                }
                else
                {
                    uiPanel = UIView.GetAView().AddUIComponent(typeof(PillarCustomizerPanelVanilla)) as PillarCustomizerPanel;
                }
                
            }
        }

        public void OnLevelUnloading()
        {
            // Restore bridge pillar defaults
            foreach (var prefab in defaultBridgePillars.Keys) 
            {
                SetPillar(prefab, PillarType.BRIDGE_PILLAR, defaultBridgePillars[prefab]);
            }
            defaultBridgePillars.Clear();

            // Restore middle pillar defaults
            foreach (var prefab in defaultMiddlePillars.Keys)
            {
                SetPillar(prefab, PillarType.MIDDLE_PILLAR, defaultMiddlePillars[prefab]);
            }
            defaultMiddlePillars.Clear();

            // Destroy GUI
            Object.Destroy(uiPanel);
        }

        public void OnReleased()
        {
            instance = null;
        }

        public BuildingInfo GetDefaultPillar(NetInfo prefab, PillarType type) 
        {
            if(type == PillarType.BRIDGE_PILLAR)
                return defaultBridgePillars[prefab];
            else
                return defaultMiddlePillars[prefab];
        }

        public void SetPillar(NetInfo prefab, PillarType type, BuildingInfo pillar) 
        {
            var netAI = prefab.m_netAI;
            
            var ta = netAI as TrainTrackBridgeAI;
            var ra = netAI as RoadBridgeAI;
            var pa = netAI as PedestrianBridgeAI;

            if (ta != null)
            {
                if (type == PillarType.BRIDGE_PILLAR) 
                    ta.m_bridgePillarInfo = pillar;
                else
                    ta.m_middlePillarInfo = pillar;
            }
            else if (ra != null)
            {
                if (type == PillarType.BRIDGE_PILLAR)
                    ra.m_bridgePillarInfo = pillar;
                else
                    ra.m_middlePillarInfo = pillar;
            }
            else if (pa != null)
            {
                pa.m_bridgePillarInfo = pillar;
            }
        }

        private void SaveDefaults(NetInfo prefab) 
        {
            BuildingInfo bridgePillar = GetActivePillar(prefab, PillarType.BRIDGE_PILLAR);
            BuildingInfo middlePillar = GetActivePillar(prefab, PillarType.MIDDLE_PILLAR);

            // save default
            if (bridgePillar != null) defaultBridgePillars.Add(prefab, bridgePillar);
            if (middlePillar != null) defaultMiddlePillars.Add(prefab, middlePillar);

            // add to list of available pillars
            if (bridgePillar != null && !pillarBuildings.Contains(bridgePillar)) pillarBuildings.Add(bridgePillar);
            if (middlePillar != null && !pillarBuildings.Contains(middlePillar)) pillarBuildings.Add(middlePillar);
        }

        public BuildingInfo GetActivePillar(NetInfo prefab, PillarType type) 
        {
            if (prefab == null) return null;
            
            var ta = prefab.m_netAI as TrainTrackBridgeAI;
            var ra = prefab.m_netAI as RoadBridgeAI;
            var pa = prefab.m_netAI as PedestrianBridgeAI;

            if (ta != null)
            {
                return (type == PillarType.BRIDGE_PILLAR) ? ta.m_bridgePillarInfo : ta.m_middlePillarInfo;
            }
            else if (ra != null)
            {
                return (type == PillarType.BRIDGE_PILLAR) ? ra.m_bridgePillarInfo : ra.m_middlePillarInfo;
            }
            else if (pa != null)
            {
                return (type == PillarType.BRIDGE_PILLAR) ? pa.m_bridgePillarInfo : null;
            }

            return null;
        }

        public List<BuildingInfo> GetAvailablePillars(NetInfo prefab, PillarType type)
        {
            if (prefab.m_netAI is TrainTrackBridgeAI || prefab.m_netAI is RoadBridgeAI || prefab.m_netAI is PedestrianBridgeAI)
            {
                if (type == PillarType.MIDDLE_PILLAR)
                {
                    var ta = prefab.m_netAI as TrainTrackBridgeAI;
                    var ra = prefab.m_netAI as RoadBridgeAI;
                    var pa = prefab.m_netAI as PedestrianBridgeAI;

                    if (ta != null)
                    {
                        if (!ta.m_doubleLength) return null;
                    }
                    else if (ra != null)
                    {
                        if (!ra.m_doubleLength) return null;
                    }
                    else if (pa != null)
                    {
                        return null;
                    }
                }
                
                var availablePillars = new List<BuildingInfo>();

                // no pillars
                availablePillars.Add(null);

                // TODO only return relevant pillars
                foreach(var pillar in pillarBuildings) 
                {
                    availablePillars.Add(pillar);
                }

                return availablePillars;
            }
            else
            {
                return null;
            }
        }

        private bool FineRoadHeightsEnabled()
        {
            foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                if (current.publishedFileID.AsUInt64 == 413678178uL && current.isEnabled) return true;
            }
            return false;
        }

        public NetInfo GetElevatedPrefab(NetInfo prefab) 
        {
            if (prefab.m_netAI is TrainTrackBridgeAI || prefab.m_netAI is RoadBridgeAI || prefab.m_netAI is PedestrianBridgeAI) 
            {
                return prefab;
            }
            else if (prefab.m_netAI is TrainTrackAI) 
            {
                return ((TrainTrackAI)prefab.m_netAI).m_elevatedInfo;
            }
            else if (prefab.m_netAI is RoadAI) 
            {
                return ((RoadAI)prefab.m_netAI).m_elevatedInfo;
            }
            else if (prefab.m_netAI is PedestrianPathAI)
            {
                return ((PedestrianPathAI)prefab.m_netAI).m_elevatedInfo;
            }
            else 
            {
                return null;
            }
        }
    }
}