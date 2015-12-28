using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public enum LanePosition
    {
        LEFT,
        RIGHT,
        MIDDLE
    }

    public static class LanePositionExtensions
    {
        public static string GetDescription(this LanePosition position)
        {
            switch (position)
            {
                case LanePosition.LEFT: return "Left";
                case LanePosition.MIDDLE: return "Middle";
                case LanePosition.RIGHT: return "Right";
                default: throw new ArgumentOutOfRangeException("position");
            }
        }
        public static bool IsCorrectSide(this LanePosition position, float lanePosition)
        {
            switch (position)
            {
                case LanePosition.LEFT: return lanePosition < 0f;
                case LanePosition.MIDDLE: return lanePosition == 0f;
                case LanePosition.RIGHT: return lanePosition > 0f;
                default: throw new ArgumentOutOfRangeException("position");
            }
        }
    }
}
