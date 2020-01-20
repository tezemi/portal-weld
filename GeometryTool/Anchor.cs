#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    /// <inheritdoc />
    /// <summary>
    /// Component that remains at the center of a geometry editor, and 
    /// moves all of the editor's elements when moved.
    /// </summary>
    public class Anchor : GeometryEditorElement
    {
        /// <summary>
        /// The center of the geometry editor, where this anchor should remain. 
        /// The center is the average of all the vertices.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                var totalVerts = Vector3.zero;
                foreach (var vert in GeometryEditor.Vertices)
                {
                    totalVerts += vert.transform.position;
                }

                var avg = totalVerts / GeometryEditor.Vertices.Count;

                return avg;
            }
        }

        protected virtual void OnDestroy()
        {
            if (GeometryEditor != null)
            {
                DestroyImmediate(GeometryEditor.gameObject);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (SceneView.lastActiveSceneView.orthographic)
            {
                Gizmos.DrawCube(ConvertToViewPoint(Center), new Vector3(Size, Size, Size));
            }
            
            Gizmos.DrawCube(Center, new Vector3(Size, Size, Size));
        }

        /// <inheritdoc />
        /// <summary>
        /// When the anchor is moved, all other elements of the editor are 
        /// also moved.
        /// </summary>
        /// <param name="amount">The amount the anchor was moved.</param>
        protected override void OnMoved(Vector3 amount)
        {
            foreach (var vertex in GeometryEditor.Vertices)
            {
                vertex.GeometryUpdated(amount);
            }

            foreach (var edge in GeometryEditor.Edges)
            {
                edge.GeometryUpdated(amount);
            }

            foreach (var face in GeometryEditor.Faces)
            {
                face.GeometryUpdated(amount);
            }

            foreach (var vertex in GeometryEditor.Vertices2D)
            {
                vertex.GeometryUpdated(amount);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// When another element of the editor is moved, the anchor is remain 
        /// in the center.
        /// </summary>
        /// <param name="amount">The amount the other element was moved.</param>
        public override void GeometryUpdated(Vector3 amount)
        {
            transform.position = Center;
            if (Settings.SnapToGrid)
            {
                SnapToGrid();
            }
        }

        /// <summary>
        /// Creates a new anchor associated with the specified geometry editor.
        /// </summary>
        /// <param name="geometryEditor">The editor to associate the anchor with.</param>
        /// <returns>The newly created anchor.</returns>
        public static Anchor Create(GeometryEditor geometryEditor)
        {
            var anchor = new GameObject("Geometry Editor", typeof(Anchor)).GetComponent<Anchor>();
            anchor.GeometryEditor = geometryEditor;

            anchor.transform.position = anchor.Center;

            anchor.SnapToGrid(); 

            anchor.PositionLastFrame = anchor.transform.position;
            
            return anchor;
        }
    }
}
#endif
