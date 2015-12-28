using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NetworkSkins.Props
{
    public class SegmentPropDef
    {
        public enum Features
        {
            NONE = 0,
            TREE_LEFT = 1,
            TREE_MIDDLE = 2,
            TREE_RIGHT = 4,
            STREET_LIGHT = 8,
            NO_DECALS = 16
        }

        public Features features = Features.NONE;

        public string treeLeft = null;
        public string treeMiddle = null;
        public string treeRight = null;
        public string streetLight = null;

        [NonSerialized]
        public TreeInfo treeLeftPrefab;

        [NonSerialized]
        public TreeInfo treeMiddlePrefab;

        [NonSerialized]
        public TreeInfo treeRightPrefab;

        [NonSerialized]
        public PropInfo streetLightPrefab;

        public SegmentPropDef() { }

        public SegmentPropDef(SegmentPropDef other)
        {
            if (other == null) return;
            
            this.features = other.features;
            this.treeLeft = other.treeLeft;
            this.treeLeftPrefab = other.treeLeftPrefab;
            this.treeMiddle = other.treeMiddle;
            this.treeMiddlePrefab = other.treeMiddlePrefab;
            this.treeRight = other.treeRight;
            this.treeRightPrefab = other.treeRightPrefab;
            this.streetLight = other.streetLight;
            this.streetLightPrefab = other.streetLightPrefab;
        }

        public override bool Equals(object obj)
        {
            SegmentPropDef def = obj as SegmentPropDef;
            if (def == null) return false;
            return Equals(def);
        }

        public bool Equals(SegmentPropDef other)
        {
            return this.features == other.features
                && this.treeLeft == other.treeLeft
                && this.treeMiddle == other.treeMiddle
                && this.treeRight == other.treeRight
                && this.streetLight == other.streetLight;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + features.GetHashCode();
            hash = hash * 31 + treeLeft.GetHashCode();
            hash = hash * 31 + treeMiddle.GetHashCode();
            hash = hash * 31 + treeRight.GetHashCode();
            hash = hash * 31 + streetLight.GetHashCode();
            return hash;
        }

        public override string ToString() 
        {
            return "[SegmentPropDef] features: " + features
                + ", treeLeftPrefab: " + (treeLeftPrefab == null ? "null" : treeLeftPrefab.name)
                + ", treeMiddlePrefab: " + (treeMiddlePrefab == null ? "null" : treeMiddlePrefab.name)
                + ", treeRightPrefab: " + (treeRightPrefab == null ? "null" : treeRightPrefab.name)
                + ", streetLightPrefab: " + (streetLightPrefab == null ? "null" : streetLightPrefab.name);
        }
    }
}
