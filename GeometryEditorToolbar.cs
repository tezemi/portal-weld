#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld
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

        protected virtual void OnEnable()
        {
            // Get settings and apply to toolbar
            Settings.SnapToGrid = EditorPrefs.GetBool("Settings.SnapToGrid", true);
            Settings.ShowMeshPreview = EditorPrefs.GetBool("Settings.ShowMeshPreview", false);
            Settings.ShowDynamicGizmos = EditorPrefs.GetBool("Settings.ShowDynamicGizmos", false);
            Settings.GridSize = EditorPrefs.GetFloat("Settings.GridSize", 1f);
            Settings.GeometryEditMode = (GeometryEditMode)EditorPrefs.GetInt("Settings.GeometryEditMode", 0);
            Settings.BaseMaterial = AssetDatabase.LoadAssetAtPath<Material>(EditorPrefs.GetString("Settings.BaseMaterial", string.Empty));
            Settings.SelectedTexture = AssetDatabase.LoadAssetAtPath<Texture>(EditorPrefs.GetString("Settings.SelectedTexture", string.Empty));
        }

        protected virtual void OnDisable()
        {
            // Save any settings changed by toolbar
            EditorPrefs.SetBool("Settings.SnapToGrid", Settings.SnapToGrid);
            EditorPrefs.SetBool("Settings.ShowMeshPreview", Settings.ShowMeshPreview);
            EditorPrefs.SetBool("Settings.ShowDynamicGizmos", Settings.ShowDynamicGizmos);
            EditorPrefs.SetFloat("Settings.GridSize", Settings.GridSize);
            EditorPrefs.SetInt("Settings.GeometryEditMode", (int)Settings.GeometryEditMode);
            EditorPrefs.SetString("Settings.BaseMaterial", AssetDatabase.GetAssetPath(Settings.BaseMaterial));
            EditorPrefs.SetString("Settings.SelectedTexture", AssetDatabase.GetAssetPath(Settings.SelectedTexture));
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

            #region Grid Size
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
            #endregion

            #region Build Geometry
            if (GUILayout.Button("Build\nGeometry", GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)))
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
