#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    /// <inheritdoc />
    /// <summary>
    /// Component used in 2D geometry editing purely for manipulation. 
    /// Unlike actual vertices, this component is not used when creating
    /// the mesh.
    /// </summary>
    public class Vertex2D : GeometryEditorElement
    {
        [HideInInspector]
        public ViewSide ViewSide;
        [HideInInspector]
        public Face Face;
        [HideInInspector]
        public Vertex2D Vertex1;
        [HideInInspector]
        public Vertex2D Vertex2;


        protected virtual void OnRenderObject()
        {
            transform.position = GetPosition();
        }

        protected override void OnDrawGizmos()
        {
            if (!ViewSide.HasFlag(Utilities.GetCurrentViewSide())) return;

            base.OnDrawGizmos();
            if (SceneView.lastActiveSceneView.orthographic)
            {
                Gizmos.DrawCube(GetRenderPosition(), new Vector3(Size, Size, Size));
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            if (Vertex1 == null || Vertex2 == null)
            {
                Face.MoveTo(transform.position);
            }
            else
            {
                Vertex1.Face.transform.position = new Vector3
                (
                    transform.position.x,
                    Vertex1.Face.transform.position.y,
                    Vertex1.Face.transform.position.z
                );

                Vertex2.Face.transform.position = new Vector3
                (
                    Vertex2.Face.transform.position.x,
                    Vertex2.Face.transform.position.y,
                    transform.position.z
                );
            }

            foreach (var vert in GeometryEditor.Vertices2D)
            {
                vert.GeometryUpdated(amount);
            }
        }

        public override void GeometryUpdated(Vector3 amount)
        {
            transform.position = GetPosition();
        }

        public Vector3 GetPosition()
        {
            if (Vertex1 == null || Vertex2 == null)
            {
                return Face.transform.position;
            }

            return (Vertex1.Face.transform.position + Vertex2.Face.transform.position) / 2f;
        }

        public Vector3 GetRenderPosition()
        {
            if (Vertex1 == null || Vertex2 == null)
            {
                return ConvertToViewPoint(Face.transform.position);
            }

            return (Vertex1.Face.transform.position + Vertex2.Face.transform.position) / 2f;
        }
        
        public static Vertex2D Create(GeometryEditor editor, Face face, ViewSide viewSide)
        {
            var vertex2D = CreateBase<Vertex2D>(editor);

            vertex2D.Face = face;
            vertex2D.ViewSide = viewSide;

            vertex2D.transform.position = vertex2D.GetPosition();
            vertex2D.PositionLastFrame = vertex2D.GetPosition();

            editor.Vertices2D.Add(vertex2D);
            vertex2D.name += $" {editor.Vertices2D.IndexOf(vertex2D)}";

            return vertex2D;
        }

        public static Vertex2D Create(GeometryEditor editor, Vertex2D vertex1, Vertex2D vertex2, ViewSide viewSide)
        {
            var vertex2D = CreateBase<Vertex2D>(editor);

            vertex2D.Vertex1 = vertex1;
            vertex2D.Vertex2 = vertex2;
            vertex2D.ViewSide = viewSide;

            vertex2D.transform.position = vertex2D.GetPosition();
            vertex2D.PositionLastFrame = vertex2D.GetPosition();

            editor.Vertices2D.Add(vertex2D);
            vertex2D.name += $" {editor.Vertices2D.IndexOf(vertex2D)}";

            return vertex2D;
        }
    }
}

#endif
