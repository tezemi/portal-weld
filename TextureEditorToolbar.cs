#if UNITY_EDITOR
using PortalWeld.TextureTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld
{
    public class TextureEditorToolbar : Toolbar
    {
        [MenuItem("Portal Weld/Texture Editor", false, 2)]
        [MenuItem("Window/Portal Weld Texture Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(TextureEditorToolbar), false, "Texture Editor", true);
            window.minSize = new Vector2(WindowMinimumSize, WindowMinimumSize);
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Texture Editor", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            #region Texture Selection
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(100f));

            Settings.BaseMaterial = (Material)EditorGUILayout.ObjectField
            (
                Settings.BaseMaterial,
                typeof(Material),
                false,
                GUILayout.Width(ButtonSize)
            );

            Settings.SelectedTexture = (Texture)EditorGUILayout.ObjectField
            (
                Settings.SelectedTexture,
                typeof(Texture),
                false,
                GUILayout.Width(ButtonSize - 18f),
                GUILayout.Height(ButtonSize - 18f)
            );

            EditorGUILayout.EndVertical();
            #endregion

            #region Texture Manipulation
            EditorGUILayout.BeginVertical(GUILayout.Width(300f));

            if (Utilities.IsSelected<EditableTexture>())
            {
                var editableTexture = Utilities.GetFromSelection<EditableTexture>();
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

            #region Apply Button
            if (GUILayout.Button("Apply\nTexture", GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize)) && Utilities.IsSelected<EditableTexture>())
            {
                foreach (var editableTexture in Utilities.GetManyFromSelection<EditableTexture>())
                {
                    editableTexture.Material.mainTexture = Settings.SelectedTexture;
                    PortalWeldCallbacks.TextureApplied?.Invoke(editableTexture.GetComponent<MeshRenderer>(), editableTexture.Material.mainTexture);
                }
            }
            #endregion

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
