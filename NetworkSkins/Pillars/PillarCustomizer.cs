using System.Collections.Generic;
using System.IO;
using ColossalFramework.Packaging;
using ICities;

namespace NetworkSkins.Pillars
{
    public class PillarCustomizer : ILoadingExtension
    {
        public static PillarCustomizer instance;
        
        private readonly List<BuildingInfo> availablePillars = new List<BuildingInfo>();

        private readonly Dictionary<NetInfo, BuildingInfo> defaultBridgePillars = new Dictionary<NetInfo, BuildingInfo>();
        private readonly Dictionary<NetInfo, BuildingInfo> defaultMiddlePillars = new Dictionary<NetInfo, BuildingInfo>();

        public void OnCreated(ILoading loading)
        {
            instance = this;
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            // Don't load if it's not a game
            if (!NetworkSkinsMod.CheckLoadMode(mode)) return;

            // Save defaults
            for(uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<NetInfo>.GetLoaded(i);

                if (prefab == null) continue;

                SaveDefaults(prefab);
            }

            availablePillars.Add(null); // no pillar = null;

            // support for custom pillars
            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetLoaded(i);

                if (prefab == null) continue;

                // only accept buildings with a basic AI
                if (prefab.m_buildingAI.GetType() != typeof(BuildingAI)) continue;

                var asset = PackageManager.FindAssetByName(prefab.name);

                var crpPath = asset?.package?.packagePath;
                if (crpPath == null) continue;

                var pillarConfigPath = Path.Combine(Path.GetDirectoryName(crpPath), "Pillar.xml");

                // TODO parse the config file

                if (File.Exists(pillarConfigPath)) 
                {
                    availablePillars.Add(prefab);
                }
            }

            // TODO Detect No Pillars and hide its "pillars" button
        }

        public void OnLevelUnloading()
        {
            // Restore bridge pillar defaults
            foreach (var prefab in defaultBridgePillars.Keys) 
            {
                SetPillar(prefab, PillarType.BridgePillar, defaultBridgePillars[prefab]);
            }
            defaultBridgePillars.Clear();

            // Restore middle pillar defaults
            foreach (var prefab in defaultMiddlePillars.Keys)
            {
                SetPillar(prefab, PillarType.MiddlePillar, defaultMiddlePillars[prefab]);
            }
            defaultMiddlePillars.Clear();
        }

        public void OnReleased()
        {
            instance = null;
        }

        public BuildingInfo GetDefaultPillar(NetInfo prefab, PillarType type) 
        {
            var map = (type == PillarType.BridgePillar) ? defaultBridgePillars : defaultMiddlePillars;
            
            BuildingInfo pillar;
            if (!map.TryGetValue(prefab, out pillar)) return null;
            return pillar;
        }

        public void SetPillar(NetInfo prefab, PillarType type, BuildingInfo pillar) 
        {
            var netAI = prefab.m_netAI;
            
            var ta = netAI as TrainTrackBridgeAI;
            var ra = netAI as RoadBridgeAI;
            var pa = netAI as PedestrianBridgeAI;

            if (ta != null)
            {
                if (type == PillarType.BridgePillar) 
                    ta.m_bridgePillarInfo = pillar;
                else
                    ta.m_middlePillarInfo = pillar;
            }
            else if (ra != null)
            {
                if (type == PillarType.BridgePillar)
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
            var bridgePillar = GetActivePillar(prefab, PillarType.BridgePillar);
            var middlePillar = GetActivePillar(prefab, PillarType.MiddlePillar);

            // save default
            if (bridgePillar != null) defaultBridgePillars.Add(prefab, bridgePillar);
            if (middlePillar != null) defaultMiddlePillars.Add(prefab, middlePillar);

            // add to list of available pillars
            if (bridgePillar != null && !availablePillars.Contains(bridgePillar)) availablePillars.Add(bridgePillar);
            if (middlePillar != null && !availablePillars.Contains(middlePillar)) availablePillars.Add(middlePillar);
        }

        public BuildingInfo GetActivePillar(NetInfo prefab, PillarType type) 
        {
            if (prefab == null) return null;
            
            var ta = prefab.m_netAI as TrainTrackBridgeAI;
            var ra = prefab.m_netAI as RoadBridgeAI;
            var pa = prefab.m_netAI as PedestrianBridgeAI;

            if (ta != null)
            {
                return (type == PillarType.BridgePillar) ? ta.m_bridgePillarInfo : ta.m_middlePillarInfo;
            }
            else if (ra != null)
            {
                return (type == PillarType.BridgePillar) ? ra.m_bridgePillarInfo : ra.m_middlePillarInfo;
            }
            else if (pa != null)
            {
                return (type == PillarType.BridgePillar) ? pa.m_bridgePillarInfo : null;
            }
            else
            {
                return null;
            }
        }

        public List<BuildingInfo> GetAvailablePillars(NetInfo prefab, PillarType pillarType)
        {
            if (prefab == null) return null;
            
            if (prefab.m_netAI is TrainTrackBridgeAI || prefab.m_netAI is RoadBridgeAI || prefab.m_netAI is PedestrianBridgeAI)
            {
                if (pillarType == PillarType.MiddlePillar)
                {
                    var ta = prefab.m_netAI as TrainTrackBridgeAI;
                    var ra = prefab.m_netAI as RoadBridgeAI;

                    if (ta != null)
                    {
                        if (!ta.m_doubleLength) return null;
                    }
                    else if (ra != null)
                    {
                        if (!ra.m_doubleLength) return null;
                    }
                    else
                    {
                        return null;
                    }
                }

                // TODO only return relevant pillars
                return availablePillars;
            }
            else
            {
                return null;
            }
        }
    }
}
