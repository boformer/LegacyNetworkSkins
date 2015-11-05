using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins
{
    public class ADebugger : Singleton<ADebugger>
    {
        // call counters
        public int a_lateUpdate = 0;
        public int a_lateUpdate2 = 0;
        public int a_renderInstance = 0;
        public int a_refreshInstance = 0;
        public int a_calculateGroupData = 0;
        public int a_populateGroupData = 0;
        public int a_updateSegmentRenderer = 0;

        // trees for the road tree randomizer (showcase)
        public TreeInfo[] trees = null;

        // serialization test
        public Props.PropCustomizer b_propCustomizer = null;
    }
}
