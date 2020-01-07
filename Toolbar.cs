#if UNITY_EDITOR
using PortalWeld.TextureTool;
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld
{
    /// <summary>
    /// Class for portal weld's toolbar window.
    /// </summary>
    public class Toolbar : EditorWindow
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
        protected float ButtonSize => position.height - 26f;
        /// <summary>
        /// Smallest size the toolbar can be.
        /// </summary>
        protected const float WindowMinimumSize = 10f;

        [MenuItem("Window/Portal Weld Toolbar")]
        [MenuItem("Portal Weld/Portal Weld Toolbar")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(Toolbar), false, "Portal Weld", true);
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
            Settings.DefaultTexture = AssetDatabase.LoadAssetAtPath<Texture>(EditorPrefs.GetString("Settings.DefaultTexture", string.Empty));
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
            EditorPrefs.SetString("Settings.DefaultTexture", AssetDatabase.GetAssetPath(Settings.DefaultTexture));
        }

        protected virtual void OnGUI()
        {
            #region Button Style
            if (UnselectedStyle == null)
            {
                UnselectedStyle = new GUIStyle("Button");
            }

            if (SelectedStyle == null)
            {
                SelectedStyle = new GUIStyle(UnselectedStyle);
                SelectedStyle.normal.background = UnselectedStyle.active.background;
            }
            #endregion

            EditorGUILayout.BeginHorizontal();

            #region Geometry Editor

            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2.5f));

            GUILayout.Label("Geometry Editor", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            #region Geometry Edit Mode Buttons
            PortalWeldToolButton("Edge\nEdit Mode", 0);
            PortalWeldToolButton("Vertex\nEdit Mode", 1);
            PortalWeldToolButton("Face\nEdit Mode", 2);
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

            #region Texture Selection
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(100f));

            Settings.BaseMaterial = (Material)EditorGUILayout.ObjectField
            (
                Settings.BaseMaterial,
                typeof(Material),
                false,
                GUILayout.Width(ButtonSize)
            );

            Settings.DefaultTexture = (Texture)EditorGUILayout.ObjectField
            (
                Settings.DefaultTexture,
                typeof(Texture),
                false,
                GUILayout.Width(ButtonSize - 18f),
                GUILayout.Height(ButtonSize - 18f)
            );

            EditorGUILayout.EndVertical();
            #endregion

            #region Build Geometry
            if (GUILayout.Button("Build\nGeometry", GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)))
            {
                GeometryEditor.Current.BuildGeometry();
            }
            #endregion

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            #endregion

            #region Texture Editor

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Texture Editor", EditorStyles.boldLabel);
            
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<EditableTexture>() != null)
            {
                var editableTexture = Selection.activeGameObject.GetComponent<EditableTexture>();
                editableTexture.Tiling = EditorGUILayout.Vector2Field("Texture Tiling", editableTexture.Tiling);
                editableTexture.Offset = EditorGUILayout.Vector2Field("Texture Offset", editableTexture.Offset);
                editableTexture.Rotation = EditorGUILayout.FloatField("Texture Rotation", editableTexture.Rotation);
            }
            else
            {
                EditorGUILayout.Vector2Field("Texture Tiling", Vector2.zero);
                EditorGUILayout.Vector2Field("Texture Offset", Vector2.zero);
                EditorGUILayout.FloatField("Texture Rotation", 0f);
            }

            EditorGUILayout.EndVertical();
            
            #endregion

            EditorGUILayout.EndHorizontal();

            // Creates a toggle button that changes the current tool to the movement tool, 
            // and also changes the geometryEditor edit mode
            void PortalWeldToolButton(string text, int index)
            {
                if (GUILayout.Button(text, (int)Settings.GeometryEditMode == index ? SelectedStyle : UnselectedStyle, GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)))
                {
                    Tools.current = Tool.Move;
                    Settings.GeometryEditMode = (GeometryEditMode)index;
                }
            }
        }
    }
}
#endif
