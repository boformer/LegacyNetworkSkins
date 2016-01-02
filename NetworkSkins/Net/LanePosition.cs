using System;
using NetworkSkins.Data;
using UnityEngine;

namespace NetworkSkins.Net
{
    public enum LanePosition
    {
        Left,
        Right,
        Middle
    }

    public static class LanePositionExtensions
    {
        public static string GetDescription(this LanePosition position)
        {
            switch (position)
            {
                case LanePosition.Left: return "Left";
                case LanePosition.Middle: return "Middle";
                case LanePosition.Right: return "Right";
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }
        public static bool IsCorrectSide(this LanePosition position, float lanePosition)
        {
            switch (position)
            {
                case LanePosition.Left: return lanePosition < 0f;
                case LanePosition.Middle: return Mathf.Approximately(lanePosition, 0f);
                case LanePosition.Right: return lanePosition > 0f;
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public static SegmentData.FeatureFlags ToTreeFeatureFlag(this LanePosition position)
        {
            switch (position)
            {
                case LanePosition.Left: return SegmentData.FeatureFlags.TreeLeft;
                case LanePosition.Middle: return SegmentData.FeatureFlags.TreeMiddle;
                case LanePosition.Right: return SegmentData.FeatureFlags.TreeRight;
                default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }
    }
}
