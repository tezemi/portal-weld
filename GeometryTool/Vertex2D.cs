#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    /// <summary>
    /// Component used in 2D geometry editing purely for manipulation. 
    /// Unlike actual vertices, this component is not used when creating
    /// the mesh.
    /// </summary>
    public class Vertex2D : GeometryEditorElement
    {
        [HideInInspector]
        public Face Face1;
        [HideInInspector]
        public Face Face2;

        protected override void OnDrawGizmos()
        {
            if (SceneView.lastActiveSceneView.orthographic)
            {
                Gizmos.DrawCube(GetPosition(), new Vector3(Size, Size, Size));
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            if (Face2 == null)
            {
                Face1.MoveTo(new Vector3
                (
                    transform.position.x,
                    Face1.transform.position.y,
                    transform.position.z
                ));
            }
            else
            {
                Face1.transform.position = new Vector3
                (
                    transform.position.x,
                    Face1.transform.position.y,
                    Face1.transform.position.z
                );

                Face2.transform.position = new Vector3
                (
                    Face2.transform.position.x,
                    Face2.transform.position.y,
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
            var sceneCamera = SceneView.lastActiveSceneView.camera;

            if (Face2 == null)
            {
                return new Vector3
                (
                    Face1.transform.position.x,
                    sceneCamera.transform.position.y - 10f, 
                    Face1.transform.position.z
                );
            }

            return new Vector3
            (
                Face1.transform.position.x,
                sceneCamera.transform.position.y - 10f,
                Face2.transform.position.z
            );
        }

        public static Vertex2D Create(GeometryEditor editor, Face face1, Face face2)
        {
            var vertex2D = CreateBase<Vertex2D>(editor);

            vertex2D.Face1 = face1;
            vertex2D.Face2 = face2;

            vertex2D.transform.position = vertex2D.GetPosition();
            vertex2D.PositionLastFrame = vertex2D.GetPosition();

            editor.Vertices2D.Add(vertex2D);
            vertex2D.name += $" {editor.Vertices2D.IndexOf(vertex2D)}";

            return vertex2D;
        }
    }
}

#endif
