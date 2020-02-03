#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using PortalWeld.TextureTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld
{
    /// <summary>
    /// Base class for portal weld toolbars.
    /// </summary>
    public abstract class Toolbar : EditorWindow
    {
        /// <summary>
        /// Style for buttons that work as toggles and are currently selected.
        /// </summary>
        protected GUIStyle SelectedStyle { get; set; }
        /// <summary>
        /// Style for buttons that work as toggles and are not selected.
        /// </summary>
        protected GUIStyle UnselectedStyle { get; set; }
        /// <summary>
        /// The width and height to use for buttons on the toolbar.
        /// </summary>
        protected float ButtonSize => position.height - 33f;
        /// <summary>
        /// Smallest size the toolbar can be.
        /// </summary>
        protected const float WindowMinimumSize = 10f;

        [MenuItem("Portal Weld/Toolbars", false, 0)]
        [MenuItem("Window/Portal Weld Toolbars")]
        public static void ShowPortalWeldWindows()
        {
            GeometryEditorToolbar.ShowWindow();
            TextureEditorToolbar.ShowWindow();
        }

        protected virtual void OnGUI()
        {
            if (UnselectedStyle == null)
            {
                UnselectedStyle = new GUIStyle("Button");
            }

            if (SelectedStyle == null)
            {
                SelectedStyle = new GUIStyle(UnselectedStyle);
                SelectedStyle.normal.background = UnselectedStyle.active.background;
            }
        }

        protected void PortalWeldToolbarButton(string text, int index)
        {
            if (GUILayout.Button(text, (int)Settings.GeometryEditMode == index ? SelectedStyle : UnselectedStyle, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)))
            {
                Tools.current = Tool.Move;
                Settings.GeometryEditMode = (GeometryEditMode)index;
            }
        }
    }
}
#endif
