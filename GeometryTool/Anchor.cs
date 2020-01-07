#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class Anchor : GeometryObject
    {
        protected override float Size => Settings.ShowDynamicGizmos ? Vector3.Distance(transform.position, Camera.current.transform.position) * DynamicGizmoScale : MaximumGizoSize;

        protected Vector3 Center
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
            DestroyImmediate(GeometryEditor.gameObject);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawCube(Center, new Vector3(Size, Size, Size));
        }

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
        }

        public override void GeometryUpdated(Vector3 difference)
        {
            transform.position = Center;
            if (Settings.SnapToGrid)
            {
                SnapToGrid();
            }
        }

        public static Anchor Create(GeometryEditor geometryEditor)
        {
            var anchor = new GameObject("Geometry Editor", typeof(Anchor)).GetComponent<Anchor>();
            anchor.GeometryEditor = geometryEditor;

            anchor.transform.position = anchor.Center;

            anchor.PositionLastFrame = anchor.transform.position;

            return anchor;
        }
    }
}
#endif
