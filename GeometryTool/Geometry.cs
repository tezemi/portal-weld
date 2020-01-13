#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    /// <summary>
    /// Component that gets attached to created geometry and holds data 
    /// about the created geometry.
    /// </summary>
    [ExecuteAlways]
    public class Geometry : MonoBehaviour
    {
        /// <summary>
        /// Struct holding vertex, edge, and face positions for this geometry.
        /// </summary>
        [HideInInspector]
        public GeometryData GeometryData;

        protected virtual void OnGUI()
        {
            // Created a new editor when geometry is selected
            if (Utilities.IsSelected(this) && (GeometryEditor.Current == null || !GeometryEditor.Current.EditMode))
            {
                GeometryEditor.Create(this);
            }
        }

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
            builtGeometry.GeometryData = new GeometryData(editor);

            return builtGeometry;
        }
    }
}
#endif
