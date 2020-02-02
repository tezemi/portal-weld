#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    public abstract class Face : GeometryEditorElement
    {
        [HideInInspector]
        public Vertex[] Vertices = new Vertex[4];
        
        public Vector3 Center
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
            if (Settings.GeometryEditMode == GeometryEditMode.Face && !SceneView.lastActiveSceneView.orthographic)
            {
                Gizmos.DrawSphere(Center, Size);
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            foreach (var vertex in Vertices)
            {
                vertex.transform.position += amount;
                if (Settings.SnapToGrid)
                {
                    vertex.SnapToGrid();
                }
            }

            Updates<Edge>();
            Updates<Face>();
            Updates<Anchor>();
            Updates<Vertex2D>();
        }

        public override void GeometryUpdated()
        {
            transform.position = Center;
        }

        public void MoveTo(Vector3 pos)
        {
            transform.position = pos;
            OnMoved(transform.position - PositionLastFrame);
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
