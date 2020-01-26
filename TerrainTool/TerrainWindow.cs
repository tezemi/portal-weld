#if UNITY_EDITOR
using PortalWeld.TextureTool;
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld.TerrainTool
{
    public class TerrainWindow : EditorWindow
    {
        public static TerrainWindow Current { get; private set; }
        
        public static void ShowTerrainWindow()
        {
            GetWindow(typeof(TerrainWindow), true, "Terrain Editor", true);
        }

        protected virtual void OnEnable()
        {
            Current = this;
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUI.BeginDisabledGroup(!Utilities.IsSelected<EditableTexture>() || Utilities.IsSelected<TerrainFace>());
            
            if (GUILayout.Button("Create Terrain"))
            {
                TerrainFace.Create(Selection.activeGameObject, TerrainSettings.Power);
                foreach (var face in Selection.activeGameObject.transform.parent.GetComponentsInChildren<EditableTexture>())
                {
                    if (face.gameObject != Selection.activeGameObject)
                    {
                        DestroyImmediate(face.gameObject);
                    }
                }

                DestroyImmediate(Selection.activeGameObject.GetComponentInParent<Geometry>());
                DestroyImmediate(Selection.activeGameObject.GetComponent<EditableTexture>());
                DestroyImmediate(Selection.activeGameObject.GetComponent<BoxCollider>());
                Selection.activeGameObject.AddComponent<MeshCollider>();
            }

            TerrainSettings.Power = EditorGUILayout.IntField("Power:", TerrainSettings.Power);
            if (TerrainSettings.Power < 1)
            {
                TerrainSettings.Power = 1;
            }
            else if (TerrainSettings.Power > 9)
            {
                TerrainSettings.Power = 9;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!Utilities.IsSelected<TerrainVertex>());

            TerrainSettings.CurrentTerrainTool = Utilities.ToggleButton("None", TerrainSettings.CurrentTerrainTool == TerrainTools.None) ? TerrainTools.None : TerrainSettings.CurrentTerrainTool;
            TerrainSettings.CurrentTerrainTool = Utilities.ToggleButton("Raise/Lower", TerrainSettings.CurrentTerrainTool == TerrainTools.RaiseLower) ? TerrainTools.RaiseLower : TerrainSettings.CurrentTerrainTool;
            TerrainSettings.CurrentTerrainTool = Utilities.ToggleButton("Smooth", TerrainSettings.CurrentTerrainTool == TerrainTools.Smooth) ? TerrainTools.Smooth : TerrainSettings.CurrentTerrainTool;
            TerrainSettings.CurrentTerrainTool = Utilities.ToggleButton("Set Height", TerrainSettings.CurrentTerrainTool == TerrainTools.SetHeight) ? TerrainTools.SetHeight : TerrainSettings.CurrentTerrainTool;
            TerrainSettings.TerrainToolSize = EditorGUILayout.FloatField("Tool Size:", TerrainSettings.TerrainToolSize);

            EditorGUI.EndDisabledGroup();

            var faces = Utilities.Get2FromSelection<TerrainFace>();
            EditorGUI.BeginDisabledGroup(faces.Item1 == null || faces.Item2 == null);

            if (GUILayout.Button("Snap Terrain") && faces.Item1 != null && faces.Item2 != null)
            {
                foreach (var vertex1 in faces.Item1.GetComponentsInChildren<TerrainVertex>())
                {
                    foreach (var vertex2 in faces.Item2.GetComponentsInChildren<TerrainVertex>())
                    {
                        if (Mathf.Abs(vertex1.transform.position.x - vertex2.transform.position.x) < 0.5f && Mathf.Abs(vertex1.transform.position.z - vertex2.transform.position.z) < 0.5f)
                        {
                            var midpoint = (vertex1.transform.position + vertex2.transform.position) / 2f;

                            vertex1.transform.position = midpoint;
                            vertex2.transform.position = midpoint;

                            faces.Item1.Rebuild();
                            faces.Item2.Rebuild();
                        }
                    }
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
