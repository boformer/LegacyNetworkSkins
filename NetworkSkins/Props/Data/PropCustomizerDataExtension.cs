using ColossalFramework.IO;
using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class PropCustomizerDataExtension : ISerializableDataExtension
    {
        // TODO use single id and ColossalFramework.IO.DataSerializer
        
        private const string SEGMENT_PROPS_DEF_ID = "NetworkSkins-PropCustomizerData";
        private const string SEGMENT_PROPS_MAP_ID = "NetworkSkins-SegmentPropsMap";
        
        private ISerializableData gameData;

        public void OnCreated(ISerializableData gameData) 
        {
            this.gameData = gameData;
        }

        public void OnLoadData()
        {
            try 
            {
                PropCustomizerData propData = null;
                
                if (gameData != null) 
                {
                    byte[] propDataBytes = gameData.LoadData(SEGMENT_PROPS_DEF_ID);

                    if (propDataBytes != null)
                    {
                        var xmlSerializer = new XmlSerializer(typeof(PropCustomizerData));
                        using (var memoryStream = new MemoryStream(propDataBytes))
                        {
                            propData = xmlSerializer.Deserialize(new MemoryStream(propDataBytes)) as PropCustomizerData;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Serializer is null, loading PropCustomizer data not possible!");
                }

                PropCustomizer.instance.data = propData;
            } 
            catch (Exception e) 
            {
                Debug.LogError("Error during load PropCustomizer data: " + e.Message);
                Debug.LogException(e);
            }
        }

        public void OnSaveData()
        {
            try
            {
                if (gameData != null) 
                {
                    if (PropCustomizer.instance.data != null)
                    {
                        byte[] propDataBytes;

                        var xmlSerializer = new XmlSerializer(typeof(PropCustomizerData));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");

                        using (var memoryStream = new MemoryStream())
                        {
                            xmlSerializer.Serialize(memoryStream, PropCustomizer.instance.data, ns);
                            propDataBytes = memoryStream.ToArray();
                        }

                        if (propDataBytes != null) 
                        {
                            gameData.SaveData(SEGMENT_PROPS_DEF_ID, propDataBytes);
                        }

                        // output for debugging
                        using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("PropCustomizerData.xml"))
                        {
                            xmlSerializer.Serialize(streamWriter, PropCustomizer.instance.data, ns);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Serializer is null, saving PropCustomizer data not possible!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error during save PropCustomizer data: " + e.Message);
                Debug.LogException(e);
            }
        }

        public void OnReleased()
        {
            gameData = null;
        }
    }
}
