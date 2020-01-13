#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public abstract class Face : GeometryEditorElement
    {
        [HideInInspector]
        public Vertex[] Vertices = new Vertex[4];
        
        protected Vector3 Center
        {
            get
            {
                var totalVerts = Vector3.zero;
                foreach (var vert in Vertices)
                {
                    totalVerts += vert.transform.position;
                }

                var avg = totalVerts / Vertices.Length;

                return avg;
            }
        }

        protected virtual void OnDestroy()
        {
            if (!GeometryEditor.IsBeingDestroyed)
            {
                GeometryEditor.Faces.Remove(this);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (Settings.GeometryEditMode == GeometryEditMode.Face)
            {
                Gizmos.DrawSphere(Center, Size);
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            GeometryEditor.Anchor.GeometryUpdated(amount);
            foreach (var vertex in Vertices)
            {
                vertex.GeometryUpdated(amount);
                if (Settings.SnapToGrid)
                {
                    vertex.SnapToGrid();
                }
            }

            foreach (var edge in GeometryEditor.Edges)
            {
                edge.GeometryUpdated(amount);
            }

            foreach (var face in GeometryEditor.Faces)
            {
                if (face != this)
                {
                    face.GeometryUpdated(amount);
                }
            }
        }

        public override void GeometryUpdated(Vector3 difference)
        {
            transform.position = Center;
        }

        protected static T CreateBaseFace<T>(GeometryEditor geometryEditor, params Vertex[] vertices) where T : Face
        {
            var face = CreateBase<T>(geometryEditor);

            face.Vertices = vertices;

            face.transform.position = face.Center;

            geometryEditor.Faces.Add(face);
            face.name += $" {geometryEditor.Faces.IndexOf(face)}";

            return face;
        }
    }
}
#endif
