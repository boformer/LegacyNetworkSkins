using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public enum NetType
    {
        // NetUtil.NET_TYPE_NAMES must always be updated when this is modified!
        UNDEFINED = -1,
        TUNNEL = 0,
        GROUND = 1,
        ELEVATED = 2,
        BRIDGE = 3
    }
}
