﻿#if UNITY_EDITOR
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

        protected virtual void OnDrawGizmos()
        {
            foreach (var edge in GeometryData.Edges)
            {
                Debug.DrawLine(edge.Vertex1, edge.Vertex2, Color.red, 0.06f);
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

        static Geometry()
        {
            // When geometry is clicked, create an editor for it
            // If anything else is clicked, delete the editor
            Selection.selectionChanged += () =>
            {
                if (Utilities.IsSelected<Geometry>() || Utilities.IsSelectedInParent<Geometry>())
                {
                    var selectedGeometry = Utilities.GetFromSelection<Geometry>() ?? Utilities.GetFromSelectionParent<Geometry>();
                     
                    if (GeometryEditor.Current == null || GeometryEditor.Current.GeometryBeingEdited != selectedGeometry || !GeometryEditor.Current.EditMode)
                    {
                        if (GeometryEditor.Current != null && GeometryEditor.Current.GeometryBeingEdited != selectedGeometry)
                        {
                            GeometryEditor.Current.Delete();
                        }

                        GeometryEditor.Create(selectedGeometry);
                    }
                }
                else
                {
                    if (GeometryEditor.Current == null || !GeometryEditor.Current.EditMode)
                        return;

                    if (Selection.activeGameObject != null && (Utilities.IsSelected<GeometryEditor>() || 
                    Utilities.IsSelected<GeometryEditorElement>() || Selection.activeGameObject.GetComponentInParent<Geometry>() != null))
                        return;
                    
                    GeometryEditor.Current.Delete();
                }
            };
        }
    }
}
#endif
