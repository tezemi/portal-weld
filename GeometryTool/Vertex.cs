#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class Vertex : GeometryEditorElement
    {
        [SerializeField]
        [HideInInspector]
        private List<Edge> _edges = new List<Edge>();

        public List<Edge> Edges
        {
            get
            {
                _edges.RemoveAll(n => n == null);

                return _edges;
            }
            protected set
            {
                _edges = value;
            }
        }

        public int Index
        {
            get
            {
                for (var i = 0; i < GeometryEditor.Vertices.Count; i++)
                {
                    if (this == GeometryEditor.Vertices[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (!GeometryEditor.IsBeingDestroyed)
            {
                var facesToDestroy = new List<Face>();
                foreach (var face in GeometryEditor.Faces)
                {
                    foreach (var vert in face.Vertices)
                    {
                        if (vert == this)
                        {
                            facesToDestroy.Add(face);
                        }
                    }
                }

                foreach (var face in facesToDestroy)
                {
                    DestroyImmediate(face);
                }

                GeometryEditor.Vertices.Remove(this);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (Settings.GeometryEditMode == GeometryEditMode.Vertex)
            {
                Gizmos.DrawSphere(transform.position, Size);
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            GeometryEditor.Anchor.GeometryUpdated(amount);
            foreach (var edge in GeometryEditor.Edges)
            {
                edge.GeometryUpdated(amount);
            }

            foreach (var face in GeometryEditor.Faces)
            {
                face.GeometryUpdated(amount);
            }
        }

        public override void GeometryUpdated(Vector3 difference)
        {
            transform.position += difference;
        }

        public bool IsConnectedTo(Vertex other)
        {
            foreach (var edge in Edges)
            {
                if (edge.Vertex1 == other || edge.Vertex2 == other)
                {
                    return true;
                }
            }

            return false;
        }

        public static Vertex Create(GeometryEditor geometryEditor, Vector3 position)
        {
            var vertex = CreateBase<Vertex>(geometryEditor);
            vertex.transform.position = position;
            geometryEditor.Vertices.Add(vertex);
            vertex.name += $" {geometryEditor.Vertices.IndexOf(vertex)}";

            vertex.PositionLastFrame = vertex.transform.position;

            return vertex; 
        }
    }
}
#endif