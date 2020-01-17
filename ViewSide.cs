#if UNITY_EDITOR
using System;

namespace PortalWeld
{
    [Flags]
    public enum ViewSide
    {
        None = 0,
        Top = 1,
        Front = 2,
        Side = 4
    }
}
#endif
