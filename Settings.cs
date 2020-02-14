#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

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
        public static bool SnapToGrid { get; set; }
        /// <summary>
        /// Whether or not to show a preview of what geometry will look like 
        /// after it's built.
        /// </summary>
        public static bool ShowMeshPreview { get; set; }
        /// <summary>
        /// If true, the user must double click faces in order to edit them.
        /// </summary>
        public static bool DoubleClickFaces { get; set; } = true;
        /// <summary>
        /// Whether or not created geometry should have a collider attached.
        /// </summary>
        public static bool IsSolid { get; set; }
        /// <summary>
        /// True if the newly created geometry should be static.
        /// </summary>
        public static bool IsStatic { get; set; }
        /// <summary>
        /// The current size of the grid to snap to.
        /// </summary>
        public static float GridSize { get; set; }
        /// <summary>
        /// Defines the scale at which all textures will appear on geometry.
        /// </summary>
        public static float TextureScale { get; set; }
        /// <summary>
        /// The current geometry edit mode.
        /// </summary>
        public static GeometryEditMode GeometryEditMode { get; set; }
        /// <summary>
        /// The layers to apply to created geometry.
        /// </summary>
        public static LayerMask GeometryLayerMask { get; set; }
        /// <summary>
        /// The data for the most recently built geometry.
        /// </summary>
        public static Vector3? LastBuiltGeometryPosition { get; set; }
        /// <summary>
        /// The material to use as a base for newly created geometry.
        /// </summary>
        public static Material BaseMaterial { get; set; }
        /// <summary>
        /// The texture to apply to newly created geometry, and also the 
        /// texture to apply when the apply button is clicked.
        /// </summary>
        public static Texture SelectedTexture { get; set; }

        static Settings()
        {
            SnapToGrid = EditorPrefs.GetBool("Settings.SnapToGrid", true);
            ShowMeshPreview = EditorPrefs.GetBool("Settings.ShowMeshPreview", false);
            IsSolid = EditorPrefs.GetBool("Settings.IsSolid", true);
            IsStatic = EditorPrefs.GetBool("Settings.IsStatic", true);

            GridSize = EditorPrefs.GetFloat("Settings.GridSize", 1f);
            TextureScale = EditorPrefs.GetFloat("Settings.TextureScale", 2f);
            GeometryEditMode = (GeometryEditMode)EditorPrefs.GetInt("Settings.GeometryEditMode", 0);
            GeometryLayerMask = EditorPrefs.GetInt("Settings.GeometryLayerMask", 0);

            BaseMaterial = Utilities.GetEditorPrefAsset<Material>("Settings.BaseMaterial", string.Empty);
            SelectedTexture = Utilities.GetEditorPrefAsset<Texture>("Settings.SelectedTexture", string.Empty);
            
            EditorApplication.quitting += SaveEditorPrefs;
            AssemblyReloadEvents.beforeAssemblyReload += SaveEditorPrefs;

            void SaveEditorPrefs()
            {
                EditorPrefs.SetBool("Settings.SnapToGrid", SnapToGrid);
                EditorPrefs.SetBool("Settings.ShowMeshPreview", ShowMeshPreview);
                EditorPrefs.SetBool("Settings.IsSolid", IsSolid);
                EditorPrefs.SetBool("Settings.IsStatic", IsStatic);

                EditorPrefs.SetFloat("Settings.GridSize", GridSize);
                EditorPrefs.SetFloat("Settings.TextureScale", TextureScale);
                EditorPrefs.SetInt("Settings.GeometryEditMode", (int)GeometryEditMode);
                EditorPrefs.SetInt("Settings.GeometryLayerMask", GeometryLayerMask);

                Utilities.SetEditorPrefAsset("Settings.BaseMaterial", BaseMaterial);
                Utilities.SetEditorPrefAsset("Settings.SelectedTexture", SelectedTexture);
            }
        }
    }
}
#endif
