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
        public Vector3 Offset;
        [HideInInspector]
        public ViewSide ViewSide;
        [HideInInspector]
        public Face Face;
        [HideInInspector]
        public Vertex2D Vertex1;
        [HideInInspector]
        public Vertex2D Vertex2;
        private float _oldCameraDistance;
        private SceneView _lastSceneView;

        protected virtual void OnRenderObject()
        {
            if (_lastSceneView == SceneView.lastActiveSceneView && _oldCameraDistance != SceneView.lastActiveSceneView.cameraDistance)
            {
                DisableOnMoved = true;
                transform.position = GetPosition();
                _oldCameraDistance = SceneView.lastActiveSceneView.cameraDistance;
            }
            else
            {
                DisableOnMoved = false;
            }

            _lastSceneView = SceneView.lastActiveSceneView;
        }

        protected override void OnDrawGizmos()
        {
            if (!ViewSide.HasFlag(Utilities.GetCurrentViewSide())) return;

            base.OnDrawGizmos();
            if (SceneView.lastActiveSceneView.orthographic)
            {
                Gizmos.DrawCube(transform.position, new Vector3(Size, Size, Size));
            }
        }

        protected override void OnMoved(Vector3 amount)
        {
            if (Vertex1 == null || Vertex2 == null)
            {
                Face.transform.position += amount;
                foreach (var vertex in Face.Vertices)
                {
                    vertex.transform.position += amount;
                    if (Settings.SnapToGrid)
                    {
                        vertex.SnapToGrid();
                    }
                }

                Updates<Face>();
                Updates<Anchor>();
                Updates<Vertex2D>();

                transform.position = GetPosition();
            }
            else
            {
                if (Vertex1.ViewSide == (ViewSide.Top | ViewSide.Front))
                {
                    //Vertex1.Face.MoveTo(new Vector3(transform.position.x, Vertex1.transform.position.y, Vertex1.transform.position.z));
                    //Vertex1.transform.position = new Vector3(transform.position.x, Vertex1.transform.position.y, Vertex1.transform.position.z);
                }

                if (Vertex2.ViewSide == (ViewSide.Top | ViewSide.Side))
                {
                    //Vertex2.Face.MoveTo(new Vector3(Vertex2.transform.position.x, Vertex2.transform.position.y, transform.position.z));
                    //Vertex2.transform.position = new Vector3(Vertex2.transform.position.x, Vertex2.transform.position.y, transform.position.z);
                }

                //Vertex1.Face.MoveTo(Vertex1.transform.position);
                //Vertex2.Face.MoveTo(Vertex2.transform.position);
            }
        }

        public override void GeometryUpdated()
        {
            transform.position = GetPosition();
        }

        public Vector3 GetPosition()
        {
            if (Vertex1 == null || Vertex2 == null)
            {
                return Face.transform.position + Offset * SceneView.lastActiveSceneView.cameraDistance * DynamicGizmoScale;
            }

            if (Vertex1.ViewSide == (ViewSide.Top | ViewSide.Front) && Vertex2.ViewSide == (ViewSide.Top | ViewSide.Side))
            {
                return new Vector3(Vertex1.transform.position.x, Vertex1.transform.position.y, Vertex2.transform.position.z) + Offset;
            }

            return GeometryEditor.Anchor.transform.position;
        }
        
        public static Vertex2D Create(GeometryEditor editor, Face face, ViewSide viewSide, Vector3 offset)
        {
            var vertex2D = CreateBase<Vertex2D>(editor);

            vertex2D.Offset = offset;
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
