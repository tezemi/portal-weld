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
        /// Whether or not the geometry editor should snap to a grid.
        /// </summary>
        public static bool SnapToGrid { get; set; } = true;
        /// <summary>
        /// Whether or not to show a preview of what geometry will look like 
        /// after it's built.
        /// </summary>
        public static bool ShowMeshPreview { get; set; }
        /// <summary>
        /// If true, geometry gizmo selectors will scale with the camera's 
        /// distance.
        /// </summary>
        public static bool ShowDynamicGizmos { get; set; }
        /// <summary>
        /// The current size of the grid to snap to.
        /// </summary>
        public static float GridSize { get; set; } = 1f;
        /// <summary>
        /// The current geometry edit mode.
        /// </summary>
        public static GeometryEditMode GeometryEditMode { get; set; }
        /// <summary>
        /// The data for the most recently built geometry.
        /// </summary>
        public static GeometryData? LastBuiltGeometryData { get; set; }
        /// <summary>
        /// The material to use as a base for newly created geometry.
        /// </summary>
        public static Material BaseMaterial { get; set; }
        /// <summary>
        /// The texture to apply to newly created geometry, and also the 
        /// texture to apply when the apply button is clicked.
        /// </summary>
        public static Texture SelectedTexture { get; set; }
    }
}
#endif
