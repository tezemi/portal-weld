using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PortalWeld.GeometryTool
{
    /// <summary>
    /// Component that gets attached to created geometry and holds data 
    /// about the created geometry.
    /// </summary>
    [ExecuteAlways]
    public class Geometry : MonoBehaviour
    {
        #if UNITY_EDITOR
        [HideInInspector]
        public GeometryEditor GeometryEditor;

        /// <summary>
        /// Creates a new geometry component on the specified game object, 
        /// using data from the specified editor.
        /// </summary>
        /// <param name="geometryGameObject">The game object to attach
        /// the component to.</param>
        /// <param name="editor">The editor to get the data from.</param>
        /// <returns>The newly created geometry component.</returns>
        public static Geometry Create(GameObject geometryGameObject, GeometryEditor editor)
        {
            var builtGeometry = geometryGameObject.AddComponent<Geometry>();
            builtGeometry.GeometryEditor = editor;

            return builtGeometry;
        }

        static Geometry()
        {
            // When geometry is clicked, create an editor for it
            // If anything else is clicked, delete the editor
            Selection.selectionChanged += () =>
            {
                if (Utilities.IsSelected<Geometry>())
                {
                    var selectedGeometry = Utilities.GetFromSelection<Geometry>();
                    selectedGeometry.GeometryEditor.gameObject.SetActive(true);
                }
            };
        }
        #endif
    }
}
