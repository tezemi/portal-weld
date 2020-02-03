#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    public class GeometryEditorToolbar : Toolbar
    {
        [MenuItem("Portal Weld/Geometry Editor", false, 1)]
        [MenuItem("Window/Portal Weld Geometry Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GeometryEditorToolbar), false, "Geometry Editor", true);
            window.minSize = new Vector2(WindowMinimumSize, WindowMinimumSize);
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Geometry Editor", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            #region Geometry Edit Mode Buttons
            PortalWeldToolbarButton("Edge\nEdit Mode", 0);
            PortalWeldToolbarButton("Vertex\nEdit Mode", 1);
            PortalWeldToolbarButton("Face\nEdit Mode", 2);
            #endregion

            #region Options
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(20f));

            if (GUILayout.Button("-"))
            {
                Settings.GridSize -= 0.5f;
            }

            Settings.GridSize = EditorGUILayout.FloatField("Grid Size", Settings.GridSize);

            if (GUILayout.Button("+"))
            {
                Settings.GridSize += 0.5f;
            }

            EditorGUILayout.EndHorizontal();

            Settings.IsStatic = EditorGUILayout.Toggle("Is Static", Settings.IsStatic);
            Settings.IsSolid = EditorGUILayout.Toggle("Is Solid", Settings.IsSolid);
            Settings.GeometryLayerMask = EditorGUILayout.LayerField("Geometry Layer Mask", Settings.GeometryLayerMask);
            
            EditorGUILayout.EndVertical();
            #endregion

            #region Build Geometry
            if (GUILayout.Button("Build\nGeometry", GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)) && GeometryEditor.Current != null && !GeometryEditor.Current.EditMode)
            {
                GeometryEditor.Current.BuildGeometry();
            }
            #endregion

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
