#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    public class Edge : GeometryEditorElement
    {
        [HideInInspector]
        public bool ShowLength;
        [HideInInspector]
        public Vertex Vertex1;
        [HideInInspector]
        public Vertex Vertex2;
        public bool HasVertices => Vertex1 != null && Vertex2 != null;
        public float Length => Vector3.Distance(Vertex1.transform.position, Vertex2.transform.position);
        protected Vector3 Midpoint => (Vertex1.transform.position + Vertex2.transform.position) / 2f;
        protected readonly Vector3 LengthTextOffset = new Vector3(0f, 1.5f, 0f);

        protected virtual void OnDestroy()
        {
            GeometryEditor.Edges.Remove(this);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (Vertex1 == null || Vertex2 == null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Gizmos.DrawLine(Vertex1.transform.position, Vertex2.transform.position);

            if (Settings.GeometryEditMode == GeometryEditMode.Edge)
            {
                Gizmos.DrawSphere((Vertex1.transform.position + Vertex2.transform.position) / 2f, Size);   
            }

            if (ShowLength)
            {
                var style = new GUIStyle { normal = { textColor = Color.green } };
                Handles.Label(transform.position + LengthTextOffset, Length.ToString(), style);
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            GeometryEditor.Anchor.GeometryUpdated(amount);

            Vertex1.GeometryUpdated(amount);
            Vertex2.GeometryUpdated(amount);

            if (Settings.SnapToGrid)
            {
                Vertex1.SnapToGrid();
                Vertex2.SnapToGrid();
            }

            foreach (var edge in GeometryEditor.Edges)
            {
                if (edge != this)
                {
                    edge.GeometryUpdated(amount);
                }
            }

            foreach (var face in GeometryEditor.Faces)
            {
                face.GeometryUpdated(amount);
            }
        }

        public override void GeometryUpdated(Vector3 difference)
        {
            transform.position = Midpoint;
        }

        public Vertex Bifurcate()
        {
            var midpointVertex = Vertex.Create(GeometryEditor, (Vertex1.transform.position + Vertex2.transform.position) / 2f);
            Create(GeometryEditor, Vertex1, midpointVertex);
            Create(GeometryEditor, midpointVertex, Vertex2);

            Face oldFace = null;
            foreach (var face in GeometryEditor.Faces)
            {
                var vertex1IsInFace = false;
                var vertex2IsInFace = false;
                foreach (var vert in face.Vertices)
                {
                    if (vert == Vertex1)
                    {
                        vertex1IsInFace = true;
                    }

                    if (vert == Vertex2)
                    {
                        vertex2IsInFace = true;
                    }
                }

                if (vertex1IsInFace && vertex2IsInFace)
                {
                    oldFace = face;
                    break;
                }
            }

            if (oldFace != null)
            {
                Face3.Create(GeometryEditor, new Triangle(Vertex1, Vertex2, midpointVertex));
            }

            DestroyImmediate(gameObject);

            return midpointVertex;
        }

        public static Edge Create(GeometryEditor geometryEditor, Vertex vertex1, Vertex vertex2)
        {
            var edge = CreateBase<Edge>(geometryEditor);
            edge.Vertex1 = vertex1;
            edge.Vertex2 = vertex2;

            edge.Vertex1.Edges.Add(edge);
            edge.Vertex2.Edges.Add(edge);

            geometryEditor.Edges.Add(edge);
            edge.name += $" {geometryEditor.Edges.IndexOf(edge)}";

            edge.transform.position = edge.Midpoint;
            edge.PositionLastFrame = edge.Midpoint;

            return edge;
        }
    }
}
#endif
