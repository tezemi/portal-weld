#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PortalWeld
{
    /// <summary>
    /// Static class that contains callbacks for various portal weld 
    /// methods. These callbacks only exist inside the editor, and as 
    /// such, any code making use of them should also be editor-only.
    /// </summary>
    public static class PortalWeldCallbacks
    {
        /// <summary>
        /// Called when a single mesh that is part of geometry is built.
        /// </summary>
        public static Action<MeshFilter> MeshBuilt { get; set; }
        /// <summary>
        /// Called when geometry is finished being built.
        /// </summary>
        public static Action<GameObject> GeometryBuilt { get; set; }
        /// <summary>
        /// Called when a texture is applied to a mesh renderer.
        /// </summary>
        public static Action<MeshRenderer, Texture> TextureApplied { get; set; }
    }
}
#endif
