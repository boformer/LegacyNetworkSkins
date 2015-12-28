using ColossalFramework;
using ColossalFramework.Math;
using NetworkSkins.Detour;
using System;
using System.Reflection;
using UnityEngine;

namespace NetworkSkins.Net
{
    public class NetManagerDetour : NetManager
    {
        private static bool deployed = false;

        private static RedirectCallsState _NetManager_CreateSegment_state;
        private static MethodInfo _NetManager_CreateSegment_original;
        private static MethodInfo _NetManager_CreateSegment_detour;

        private static RedirectCallsState _NetManager_ReleaseSegment_state;
        private static MethodInfo _NetManager_ReleaseSegment_original;
        private static MethodInfo _NetManager_ReleaseSegment_detour;

        public static void Deploy()
        {
            if (!deployed)
            {
                _NetManager_CreateSegment_original = typeof(NetManager).GetMethod("CreateSegment", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_CreateSegment_detour = typeof(NetManagerDetour).GetMethod("CreateSegment", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_CreateSegment_state = RedirectionHelper.RedirectCalls(_NetManager_CreateSegment_original, _NetManager_CreateSegment_detour);

                _NetManager_ReleaseSegment_original = typeof(NetManager).GetMethod("ReleaseSegment", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_ReleaseSegment_detour = typeof(NetManagerDetour).GetMethod("ReleaseSegment", BindingFlags.Instance | BindingFlags.Public);
                _NetManager_ReleaseSegment_state = RedirectionHelper.RedirectCalls(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_detour);

                deployed = true;
            }
        }

        public static void Revert()
        {
            if (deployed)
            {
                RedirectionHelper.RevertRedirect(_NetManager_CreateSegment_original, _NetManager_CreateSegment_state);
                _NetManager_CreateSegment_original = null;
                _NetManager_CreateSegment_detour = null;

                RedirectionHelper.RevertRedirect(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_state);
                _NetManager_ReleaseSegment_original = null;
                _NetManager_ReleaseSegment_detour = null;

                deployed = false;
            }
        }

        // reset both on CreateSegment call by CreateNode
        private ushort SplitSegment_releasedSegment = 0; 
        private ushort MoveMiddleNode_releasedSegment = 0;

        public new bool CreateSegment(out ushort segment, ref Randomizer randomizer, NetInfo info, ushort startNode, ushort endNode, Vector3 startDirection, Vector3 endDirection, uint buildIndex, uint modifiedIndex, bool invert)
        {
            RedirectionHelper.RevertRedirect(_NetManager_CreateSegment_original, _NetManager_CreateSegment_state);
            bool success = NetManager.instance.CreateSegment(out segment, ref randomizer, info, startNode, endNode, startDirection, endDirection, buildIndex, modifiedIndex, invert);
            RedirectionHelper.RedirectCalls(_NetManager_CreateSegment_original, _NetManager_CreateSegment_detour);

            if (success)
            {
                string caller = new System.Diagnostics.StackFrame(1).GetMethod().Name;

                switch (caller)
                {
                    case "CreateNode":
                        string caller2 = new System.Diagnostics.StackFrame(2).GetMethod().Name;
                        if (caller2 == "CreateNode") // check that caller was called by NetTool
                        {
                            string caller3Type = new System.Diagnostics.StackFrame(3).GetMethod().DeclaringType.Name;
                            if (caller3Type == "NetTool") // new segment created by user, apply selected style
                            {
                                NetEventManager.instance.PostCreateEvent(segment);

                                // Delete data of splitted segment
                                if (SplitSegment_releasedSegment > 0) NetEventManager.instance.PostReleaseEvent(SplitSegment_releasedSegment);

                                SplitSegment_releasedSegment = 0;
                                MoveMiddleNode_releasedSegment = 0;
                            }
                        }
                        break;
                    case "MoveMiddleNode": // segment that was modified because user added network, apply style of previous segment
                        if (MoveMiddleNode_releasedSegment > 0)
                        {
                            NetEventManager.instance.PostTransferDataEvent(MoveMiddleNode_releasedSegment, segment);

                            // Delete data of previous segment
                            NetEventManager.instance.PostReleaseEvent(MoveMiddleNode_releasedSegment);
                            MoveMiddleNode_releasedSegment = 0;
                        }
                        break;
                    case "SplitSegment": // segment that was split by new node, apply style of previous segment
                        if (SplitSegment_releasedSegment > 0)
                        {
                            NetEventManager.instance.PostTransferDataEvent(SplitSegment_releasedSegment, segment);
                        }
                        break;
                    default: // unknown caller, ignore
                        break;
                }
            }

            return success;
        }

        public new void ReleaseSegment(ushort segment, bool keepNodes)
        {
            string caller = new System.Diagnostics.StackFrame(1).GetMethod().Name;

            switch (caller)
            {
                case "MoveMiddleNode": // segment that was modified because user added network, keep data until replacement segments were created
                    MoveMiddleNode_releasedSegment = segment; // save segment id
                    break;
                case "SplitSegment": // segment that was split by new node, keep data until replacement segments were created
                    SplitSegment_releasedSegment = segment; // save segment id
                    break;
                case "DeleteSegmentImpl": // segment deleted with bulldozer by user, delete data
                default: // unknown caller, delete data
                    NetEventManager.instance.PostReleaseEvent(segment);
                    break;
            }

            RedirectionHelper.RevertRedirect(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_state);
            NetManager.instance.ReleaseSegment(segment, keepNodes);
            RedirectionHelper.RedirectCalls(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_detour);
        }
    }
}
