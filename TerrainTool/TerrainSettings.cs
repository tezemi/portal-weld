#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld.TerrainTool
{
    public static class TerrainSettings
    {
        public static int Power { get; set; }
        public static float TerrainToolSize { get; set; }
        public static TerrainTools CurrentTerrainTool { get; set; }

        static TerrainSettings()
        {
            Power = EditorPrefs.GetInt("TerrainSettings.Power", 3);
            TerrainToolSize = EditorPrefs.GetFloat("TerrainSettings.TerrainToolSize", 1.5f);
            CurrentTerrainTool = (TerrainTools)EditorPrefs.GetInt("TerrainSettings.CurrentTerrainTool", 0);
            
            EditorApplication.quitting += SaveEditorPrefs;
            AssemblyReloadEvents.beforeAssemblyReload += SaveEditorPrefs;

            void SaveEditorPrefs()
            {
                EditorPrefs.SetInt("TerrainSettings.Power", Power);
                EditorPrefs.SetFloat("TerrainSettings.TerrainToolSize", TerrainToolSize);
                EditorPrefs.SetInt("TerrainSettings.CurrentTerrainTool", (int)CurrentTerrainTool);
            }
        }
    }
}
#endif
