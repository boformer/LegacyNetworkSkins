using System;
using System.Reflection;
using ColossalFramework;
using NetworkSkins.Detour;
using UnityEngine;

namespace NetworkSkins.Props
{
    public class NetManagerDetour : NetManager
    {
        private static bool deployed = false;

        private static RedirectCallsState _NetManager_UpdateSegmentRenderer_state;
        private static MethodInfo _NetManager_UpdateSegmentRenderer_original;
        private static MethodInfo _NetManager_UpdateSegmentRenderer_detour;

        public static void Deploy()
        {
            if (!deployed)
            {
                // NetManager.UpdateSegmentRenderer 

                _NetManager_UpdateSegmentRenderer_original = typeof(NetManager).GetMethod("UpdateSegmentRenderer", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_UpdateSegmentRenderer_detour = typeof(NetManagerDetour).GetMethod("UpdateSegmentRenderer", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_UpdateSegmentRenderer_state = RedirectionHelper.RedirectCalls(_NetManager_UpdateSegmentRenderer_original, _NetManager_UpdateSegmentRenderer_detour);

                deployed = true;
            }
        }

        public static void Revert()
        {
            if (deployed)
            {
                RedirectionHelper.RevertRedirect(_NetManager_UpdateSegmentRenderer_original, _NetManager_UpdateSegmentRenderer_state);
                _NetManager_UpdateSegmentRenderer_original = null;
                _NetManager_UpdateSegmentRenderer_detour = null;

                deployed = false;
            }
        }

        public new void UpdateSegmentRenderer(ushort segment, bool updateGroup)
        {
            if (this.m_segments.m_buffer[(int)segment].m_flags == NetSegment.Flags.None)
            {
                return;
            }
            Singleton<RenderManager>.instance.UpdateInstance((uint)(49152 + segment));
            if (updateGroup)
            {
                var info = this.m_segments.m_buffer[(int)segment].Info;
                if (info == null)
                {
                    return;
                }
                var startNode = this.m_segments.m_buffer[(int)segment].m_startNode;
                var endNode = this.m_segments.m_buffer[(int)segment].m_endNode;
                var position = this.m_nodes.m_buffer[(int)startNode].m_position;
                var position2 = this.m_nodes.m_buffer[(int)endNode].m_position;
                var vector = (position + position2) * 0.5f;
                var num = Mathf.Clamp((int)(vector.x / 64f + 135f), 0, 269);
                var num2 = Mathf.Clamp((int)(vector.z / 64f + 135f), 0, 269);
                var x = num * 45 / 270;
                var z = num2 * 45 / 270;
                var num3 = info.m_netLayers;
                if (info.m_lanes != null)
                {
                    var num4 = this.m_segments.m_buffer[(int)segment].m_lanes;
                    var num5 = 0;
                    while (num5 < info.m_lanes.Length && num4 != 0u)
                    {
                        var laneProps = info.m_lanes[num5].m_laneProps;
                        if (laneProps != null && laneProps.m_props != null)
                        {
                            var num6 = laneProps.m_props.Length;
                            for (var i = 0; i < num6; i++)
                            {
                                var prop = laneProps.m_props[i];
                                if (prop.m_finalProp != null)
                                {
                                    num3 |= 1 << prop.m_finalProp.m_prefabDataLayer;
                                    if (prop.m_finalProp.m_effectLayer != -1)
                                    {
                                        num3 |= 1 << prop.m_finalProp.m_effectLayer;
                                    }
                                }
                                if (prop.m_finalTree != null)
                                {
                                    num3 |= 1 << prop.m_finalTree.m_prefabDataLayer;
                                }
                            }
                        }
                        num4 = this.m_lanes.m_buffer[(int)((UIntPtr)num4)].m_nextLane;
                        num5++;
                    }
                }
                for (var j = 0; j < 32; j++)
                {
                    if ((num3 & 1 << j) != 0)
                    {
                        Singleton<RenderManager>.instance.UpdateGroup(x, z, j);
                    }
                }
            }
        }
    }
}
