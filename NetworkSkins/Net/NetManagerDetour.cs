using System;
using System.Reflection;
using ColossalFramework.Math;
using NetworkSkins.Data;
using NetworkSkins.Detour;
using UnityEngine;

namespace NetworkSkins.Net
{
    public class NetManagerDetour : NetManager
    {
        public delegate void SegmentTransferDataEventHandler(ushort oldSegment, ushort newSegment);
        public static event SegmentTransferDataEventHandler EventSegmentTransferData;

        public delegate void SegmentCreateEventHandler(ushort segment);
        public static event SegmentCreateEventHandler EventSegmentCreate;

        public delegate void SegmentReleaseEventHandler(ushort segment);
        public static event SegmentReleaseEventHandler EventSegmentRelease;

        private static bool deployed;

        private static RedirectCallsState _NetManager_CreateSegment_state;
        private static MethodInfo _NetManager_CreateSegment_original;
        private static MethodInfo _NetManager_CreateSegment_detour;

        private static RedirectCallsState _NetManager_ReleaseSegment_state;
        private static MethodInfo _NetManager_ReleaseSegment_original;
        private static MethodInfo _NetManager_ReleaseSegment_detour;

        public static void Deploy()
        {
            if (deployed) return;

            _NetManager_CreateSegment_original = typeof(NetManager).GetMethod("CreateSegment", BindingFlags.Instance | BindingFlags.Public);
            _NetManager_CreateSegment_detour = typeof(NetManagerDetour).GetMethod("CreateSegment", BindingFlags.Instance | BindingFlags.Public);
            _NetManager_CreateSegment_state = RedirectionHelper.RedirectCalls(_NetManager_CreateSegment_original, _NetManager_CreateSegment_detour);

            _NetManager_ReleaseSegment_original = typeof(NetManager).GetMethod("ReleaseSegment", BindingFlags.Instance | BindingFlags.Public);
            _NetManager_ReleaseSegment_detour = typeof(NetManagerDetour).GetMethod("ReleaseSegment", BindingFlags.Instance | BindingFlags.Public);
            _NetManager_ReleaseSegment_state = RedirectionHelper.RedirectCalls(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_detour);

            deployed = true;
        }

        public static void Revert()
        {
            if (!deployed) return;

            RedirectionHelper.RevertRedirect(_NetManager_CreateSegment_original, _NetManager_CreateSegment_state);
            _NetManager_CreateSegment_original = null;
            _NetManager_CreateSegment_detour = null;

            RedirectionHelper.RevertRedirect(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_state);
            _NetManager_ReleaseSegment_original = null;
            _NetManager_ReleaseSegment_detour = null;

            deployed = false;
        }

        // reset both on CreateSegment call by CreateNode
        private ushort SplitSegment_releasedSegment; 
        private ushort MoveMiddleNode_releasedSegment;

        public new bool CreateSegment(out ushort segment, ref Randomizer randomizer, NetInfo info, ushort startNode, ushort endNode, Vector3 startDirection, Vector3 endDirection, uint buildIndex, uint modifiedIndex, bool invert)
        {
            // Call original method
            RedirectionHelper.RevertRedirect(_NetManager_CreateSegment_original, _NetManager_CreateSegment_state);
            var success = NetManager.instance.CreateSegment(out segment, ref randomizer, info, startNode, endNode, startDirection, endDirection, buildIndex, modifiedIndex, invert);
            RedirectionHelper.RedirectCalls(_NetManager_CreateSegment_original, _NetManager_CreateSegment_detour);

            var caller = new System.Diagnostics.StackFrame(1).GetMethod().Name;
            //Debug.Log("CreateSegment (" + info.name + ") called by " + caller);

            switch (caller)
            {
                case "CreateNode":

                    var caller2 = new System.Diagnostics.StackFrame(2).GetMethod().Name;
                    //Debug.Log("... called by " + caller2);

                    if (caller2 == "CreateNode") // check that caller was called by NetTool
                    {
                        var caller3Type = new System.Diagnostics.StackFrame(3).GetMethod().DeclaringType?.Name;
                        //Debug.Log("... called by " + caller3Type);

                        if (caller3Type != null && caller3Type.StartsWith("NetTool", StringComparison.Ordinal)) // new segment created by user, apply selected style
                        // use StartsWith to cover NetToolFine from FineRoadHeights, and other possible NetTools
                        {
                            if (success) EventSegmentCreate?.Invoke(segment);

                            // Delete data of deleted segments
                            if (MoveMiddleNode_releasedSegment > 0) EventSegmentRelease?.Invoke(MoveMiddleNode_releasedSegment);
                            if (SplitSegment_releasedSegment > 0) EventSegmentRelease?.Invoke(SplitSegment_releasedSegment);

                            SplitSegment_releasedSegment = 0;
                            MoveMiddleNode_releasedSegment = 0;
                        }
                    }
                    else if (caller2 == "LoadPaths") // segment created because user placed building with integrated networks
                    {
                        // TODO SementDataManager should not appear here. Instead, add argument to CreateEvent!
                        if (SegmentDataManager.Instance.AssetMode && success) EventSegmentCreate?.Invoke(segment);
                    }
                    break;

                case "MoveMiddleNode": // segment that was modified because user added network, apply style of previous segment

                    if (MoveMiddleNode_releasedSegment > 0)
                    {
                        if (success) EventSegmentTransferData?.Invoke(MoveMiddleNode_releasedSegment, segment);

                        // Delete data of previous segment
                        EventSegmentRelease?.Invoke(MoveMiddleNode_releasedSegment);
                        MoveMiddleNode_releasedSegment = 0;
                    }
                    break;

                case "SplitSegment": // segment that was split by new node, apply style of previous segment

                    if (SplitSegment_releasedSegment > 0)
                    {
                        if (success) EventSegmentTransferData?.Invoke(SplitSegment_releasedSegment, segment);
                    }
                    break;

                default: // unknown caller, ignore
                    break;
            }

            return success;
        }

        public new void ReleaseSegment(ushort segment, bool keepNodes)
        {
            var caller = new System.Diagnostics.StackFrame(1).GetMethod().Name;
            //Debug.Log("ReleaseSegment (" + NetManager.instance.m_segments.m_buffer[segment].Info.name + ") called by " + caller);

            switch (caller)
            {
                case "MoveMiddleNode": // segment that was modified because user added network, keep data until replacement segments were created

                    // Delete data of last moved segment
                    if (MoveMiddleNode_releasedSegment > 0) EventSegmentRelease?.Invoke(MoveMiddleNode_releasedSegment);

                    // Save segment id
                    MoveMiddleNode_releasedSegment = segment; 
                    break;

                case "SplitSegment": // segment that was split by new node, keep data until replacement segments were created

                    // Delete data of last splitted segment
                    if (SplitSegment_releasedSegment > 0) EventSegmentRelease?.Invoke(SplitSegment_releasedSegment);

                    // Save segment id
                    SplitSegment_releasedSegment = segment; 
                    break;

                case "DeleteSegmentImpl": // segment deleted with bulldozer by user, delete data
                case "ReleasePaths": // segment deleted because user bulldozed building with integrated networks, delete data
                default: // unknown caller, delete data

                    EventSegmentRelease?.Invoke(segment);
                    break;
            }

            // Call original method
            RedirectionHelper.RevertRedirect(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_state);
            NetManager.instance.ReleaseSegment(segment, keepNodes);
            RedirectionHelper.RedirectCalls(_NetManager_ReleaseSegment_original, _NetManager_ReleaseSegment_detour);
        }
    }
}
