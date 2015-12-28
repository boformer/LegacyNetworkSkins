using ColossalFramework;

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

        // serialization test
        public Props.PropCustomizer b_propCustomizer = null;
    }
}
