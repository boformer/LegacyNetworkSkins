using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ColossalFramework.IO;
using ColossalFramework.Packaging;
using ColossalFramework.UI;
using ICities;
using NetworkSkins.Detour;
using UnityEngine;

namespace NetworkSkins.LightPropEnabler
{
    public class LightPropEnabler : LoadingExtensionBase
    {
        public HashSet<string> lightPropsDefParseErrors;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            RenderManagerDetour.EventUpdateData += UpdateData;
        }

        public override void OnReleased()
        {
            base.OnReleased();

            RenderManagerDetour.EventUpdateData -= UpdateData;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (lightPropsDefParseErrors?.Count > 0)
            {
                var errorMessage = lightPropsDefParseErrors.Aggregate("Error while parsing light-prop definition file(s). Contact the author of the assets. \n" + "List of errors:\n", (current, error) => current + (error + '\n'));

                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Network Skins", errorMessage, true);
            }

            lightPropsDefParseErrors = null;
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {
            try
            {

                lightPropsDefParseErrors = new HashSet<string>();
                var checkedPaths = new List<string>();

                for (uint i = 0; i < PrefabCollection<PropInfo>.LoadedCount(); i++)
                {
                    var prefab = PrefabCollection<PropInfo>.GetLoaded(i);

                    if (prefab == null) continue;

                    // search for LightPropDefinition.xml
                    var asset = PackageManager.FindAssetByName(prefab.name);

                    var crpPath = asset?.package?.packagePath;
                    if (crpPath == null) continue;

                    var lightPropsDefPath = Path.Combine(Path.GetDirectoryName(crpPath) ?? "",
                        "LightPropsDefinition.xml");

                    // skip files which were already parsed
                    if (checkedPaths.Contains(lightPropsDefPath)) continue;
                    checkedPaths.Add(lightPropsDefPath);

                    if (!File.Exists(lightPropsDefPath)) continue;

                    LightPropsDefinition lightPropsDef = null;

                    var xmlSerializer = new XmlSerializer(typeof (LightPropsDefinition));
                    try
                    {
                        using (var streamReader = new System.IO.StreamReader(lightPropsDefPath))
                        {
                            lightPropsDef = xmlSerializer.Deserialize(streamReader) as LightPropsDefinition;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        lightPropsDefParseErrors.Add(asset.package.packageName + " - " + e.Message);
                        continue;
                    }

                    if (lightPropsDef?.Props == null || lightPropsDef.Props.Count == 0)
                    {
                        lightPropsDefParseErrors.Add(asset.package.packageName + " - lightPropDef is null or empty.");
                        continue;
                    }

                    foreach (var propDef in lightPropsDef.Props)
                    {
                        if (propDef?.Name == null)
                        {
                            lightPropsDefParseErrors.Add(asset.package.packageName + " - Prop name missing.");
                            continue;
                        }

                        var propDefPrefab = FindProp(propDef.Name, asset.package.packageName);

                        if (propDefPrefab == null)
                        {
                            lightPropsDefParseErrors.Add(asset.package.packageName + " - Prop with name " + propDef.Name +
                                                         " not loaded.");
                            continue;
                        }

                        if (propDef.Effects == null || propDef.Effects.Count == 0)
                        {
                            lightPropsDefParseErrors.Add(asset.package.packageName + " - No effects specified for " +
                                                         propDef.Name + ".");
                            continue;
                        }

                        var effects = new List<PropInfo.Effect>();

                        foreach (var effectDef in propDef.Effects)
                        {
                            if (effectDef?.Name == null)
                            {
                                lightPropsDefParseErrors.Add(propDef.Name + " - Effect name missing.");
                                continue;
                            }

                            var effectPrefab = EffectCollection.FindEffect(effectDef.Name);

                            if (effectPrefab == null)
                            {
                                lightPropsDefParseErrors.Add(propDef.Name + " - Effect with name " + effectDef.Name +
                                                             " not loaded.");
                                continue;
                            }

                            if (effectDef.Position == null)
                            {
                                lightPropsDefParseErrors.Add(propDef.Name + " - Effect position not set.");
                                continue;
                            }

                            if (effectDef.Direction == null)
                            {
                                lightPropsDefParseErrors.Add(propDef.Name + " - Effect direction not set.");
                                continue;
                            }

                            var effect = new PropInfo.Effect
                            {
                                m_effect = effectPrefab,
                                m_position = effectDef.Position.ToUnityVector(),
                                m_direction = effectDef.Direction.ToUnityVector().normalized
                            };

                            effects.Add(effect);
                        }

                        if (effects.Count == 0)
                        {
                            lightPropsDefParseErrors.Add("No effects specified for " + propDef.Name + ".");
                            continue;
                        }

                        propDefPrefab.m_effects = effects.ToArray();

                        // taken from PropInfo.InitializePrefab
                        if (propDefPrefab.m_effects != null)
                        {
                            propDefPrefab.m_hasEffects = (propDefPrefab.m_effects.Length != 0);
                            for (var j = 0; j < propDefPrefab.m_effects.Length; j++)
                            {
                                if (propDefPrefab.m_effects[j].m_effect == null) continue;

                                propDefPrefab.m_effects[j].m_effect.InitializeEffect();
                                var layer = propDefPrefab.m_effects[j].m_effect.GroupLayer();
                                if (layer != -1)
                                {
                                    propDefPrefab.m_effectLayer = layer;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static PropInfo FindProp(string prefabName, string packageName)
        {
            var prefab = PrefabCollection<PropInfo>.FindLoaded(prefabName) ??
                         PrefabCollection<PropInfo>.FindLoaded(prefabName + "_Data") ??
                         PrefabCollection<PropInfo>.FindLoaded(PathEscaper.Escape(prefabName) + "_Data") ??
                         PrefabCollection<PropInfo>.FindLoaded(packageName + "." + prefabName + "_Data") ??
                         PrefabCollection<PropInfo>.FindLoaded(packageName + "." + PathEscaper.Escape(prefabName) + "_Data");

            return prefab;
        }
    }

    public class LightPropsDefinition
    {
        public List<Prop> Props { get; set; }

        public LightPropsDefinition()
        {
            Props = new List<Prop>();
        }

        public class Prop
        {
            [XmlAttribute("name"), DefaultValue(null)]
            public string Name { get; set; }

            public List<Effect> Effects { get; set; }

            public Prop()
            {
                Effects = new List<Effect>();
            }
        }

        public class Effect
        {
            [XmlAttribute("name"), DefaultValue(null)]
            public string Name { get; set; }

            public Vector Position { get; set; }
            public Vector Direction { get; set; }
        }

        public class Vector
        {
            [XmlAttribute("x"), DefaultValue(0f)]
            public float X { get; set; }

            [XmlAttribute("y"), DefaultValue(0f)]
            public float Y { get; set; }

            [XmlAttribute("z"), DefaultValue(0f)]
            public float Z { get; set; }

            public Vector3 ToUnityVector()
            {
                return new Vector3(X, Y, Z);
            }
        }
    }
}
