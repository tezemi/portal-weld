#if UNITY_EDITOR
using UnityEditor;
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

        protected virtual void Awake()
        {
            // Create a new editor when geometry is selected
            Selection.selectionChanged = () =>
            {
                if (Utilities.IsSelected(this) && (GeometryEditor.Current == null || !GeometryEditor.Current.EditMode))
                {
                    GeometryEditor.Create(this);
                }
                //else if (Selection.activeGameObject == null || GeometryEditor.Current != null && GeometryEditor.Current.GeometryBeingEdited == this && 
                //!Selection.activeGameObject.HasComponent<GeometryEditorElement>() && !Selection.activeGameObject.HasComponent<Geometry>() &&
                //Selection.activeGameObject.GetComponentInParent<Geometry>() != null)
                //{
                //    GeometryEditor.Current.Delete();
                //}
            };
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

        static Geometry()
        {
            Selection.selectionChanged = () =>
            {
                if (Utilities.IsSelected<Geometry>() && (GeometryEditor.Current == null || !GeometryEditor.Current.EditMode))
                {
                    GeometryEditor.Create(Utilities.GetFromSelection<Geometry>());
                }


                //else if (Selection.activeGameObject == null || GeometryEditor.Current != null && GeometryEditor.Current.GeometryBeingEdited == this && 
                //!Selection.activeGameObject.HasComponent<GeometryEditorElement>() && !Selection.activeGameObject.HasComponent<Geometry>() &&
                //Selection.activeGameObject.GetComponentInParent<Geometry>() != null)
                //{
                //    GeometryEditor.Current.Delete();
                //}
            };
        }
    }
}
#endif
