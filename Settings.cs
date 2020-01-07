#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using UnityEngine;

namespace PortalWeld
{
    /// <summary>
    /// Static class containing settings for portal weld. Settings are 
    /// saved in the editor prefs via the portal weld menu.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Whether or not the geometryEditor editor should snap to a grid.
        /// </summary>
        public static bool SnapToGrid { get; set; } = true;
        /// <summary>
        /// Whether or not to show a preview of what geometryEditor will look like 
        /// after it's built.
        /// </summary>
        public static bool ShowMeshPreview { get; set; }
        /// <summary>
        /// If true, geometryEditor gizmo selectors will scale with the camera's 
        /// distance.
        /// </summary>
        public static bool ShowDynamicGizmos { get; set; }
        /// <summary>
        /// The current size of the grid to snap to.
        /// </summary>
        public static float GridSize { get; set; } = 1f;
        /// <summary>
        /// The current geometryEditor edit mode.
        /// </summary>
        public static GeometryEditMode GeometryEditMode { get; set; }
        /// <summary>
        /// The material to use as a base for newly created geometryEditor.
        /// </summary>
        public static Material BaseMaterial { get; set; }
        /// <summary>
        /// The texture to apply to newly created geometryEditor.
        /// </summary>
        public static Texture DefaultTexture { get; set; }
    }
}
#endif
