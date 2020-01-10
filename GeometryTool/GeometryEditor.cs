#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using PortalWeld.TextureTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    [ExecuteAlways]
    public class GeometryEditor : MonoBehaviour
    {
        [HideInInspector]
        public bool IsBeingDestroyed;
        [HideInInspector]
        public GeometryEditType EditType;
        [HideInInspector]
        public Anchor Anchor;
        [HideInInspector]
        public MeshPreview MeshPreview;
        [HideInInspector]
        public BuiltGeometry EditingGeometry;
        [HideInInspector]
        public List<Face> Faces = new List<Face>();
        [HideInInspector]
        public List<Edge> Edges = new List<Edge>();
        [HideInInspector]
        public List<Vertex> Vertices = new List<Vertex>();
        private static GeometryEditor _current;

        public static GeometryEditor Current
        {
            get
            {
                if (_current == null && FindObjectOfType<GeometryEditor>() != null)
                {
                    _current = FindObjectOfType<GeometryEditor>();
                }

                return _current;
            }
            private set
            {
                _current = value;
            }
        }

        protected virtual void Awake()
        {
            if (Current != null && Current != this)
            {
                DestroyImmediate(Current.Anchor.gameObject);
            }

            Current = this;
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        protected virtual void OnDestroy()
        {
            IsBeingDestroyed = true;

            var cleanup = new List<GameObject>();
            foreach (var edge in Edges)
            {
                cleanup.Add(edge.gameObject);
            }

            foreach (var vert in Vertices)
            {
                cleanup.Add(vert.gameObject);
            }

            foreach (var face in Faces)
            {
                cleanup.Add(face.gameObject);
            }

            foreach (var clean in cleanup)
            {
                DestroyImmediate(clean);
            }

            DestroyImmediate(MeshPreview.gameObject);
        }

        protected virtual void OnApplicationQuit()
        {
            OnDestroy();
        }

        protected void SetupCubeEditing()
        {
            const float cubeSize = 3f;

            // GeometryEditor always starts as a cube
            // This creates eight vertices in a cube shape
            Vertex.Create(this, transform.position + new Vector3(cubeSize, cubeSize, cubeSize));
            Vertex.Create(this, transform.position + new Vector3(-cubeSize, cubeSize, cubeSize));
            Vertex.Create(this, transform.position + new Vector3(cubeSize, cubeSize, -cubeSize));
            Vertex.Create(this, transform.position + new Vector3(-cubeSize, cubeSize, -cubeSize));

            Vertex.Create(this, transform.position + new Vector3(cubeSize, -cubeSize, cubeSize));
            Vertex.Create(this, transform.position + new Vector3(-cubeSize, -cubeSize, cubeSize));
            Vertex.Create(this, transform.position + new Vector3(cubeSize, -cubeSize, -cubeSize));
            Vertex.Create(this, transform.position + new Vector3(-cubeSize, -cubeSize, -cubeSize));

            // Create the edges of the cube
            Edge.Create(this, Vertices[0], Vertices[1]).ShowLength = true;
            Edge.Create(this, Vertices[0], Vertices[2]).ShowLength = true;
            Edge.Create(this, Vertices[1], Vertices[3]);
            Edge.Create(this, Vertices[2], Vertices[3]);

            Edge.Create(this, Vertices[4], Vertices[5]);
            Edge.Create(this, Vertices[4], Vertices[6]);
            Edge.Create(this, Vertices[5], Vertices[7]);
            Edge.Create(this, Vertices[6], Vertices[7]);

            Edge.Create(this, Vertices[0], Vertices[4]).ShowLength = true;
            Edge.Create(this, Vertices[1], Vertices[5]);
            Edge.Create(this, Vertices[2], Vertices[6]);
            Edge.Create(this, Vertices[3], Vertices[7]);

            // Create the face objects
            // Order of vertices within triangles is important, defines normals, UVs
            Face4.Create(this, new Triangle(Vertices[0], Vertices[2], Vertices[1]), new Triangle(Vertices[2], Vertices[3], Vertices[1]));   // top
            Face4.Create(this, new Triangle(Vertices[6], Vertices[4], Vertices[7]), new Triangle(Vertices[4], Vertices[5], Vertices[7]));   // bottom

            Face4.Create(this, new Triangle(Vertices[6], Vertices[2], Vertices[4]), new Triangle(Vertices[2], Vertices[0], Vertices[4]));   // front
            Face4.Create(this, new Triangle(Vertices[5], Vertices[1], Vertices[7]), new Triangle(Vertices[1], Vertices[3], Vertices[7]));   // back

            Face4.Create(this, new Triangle(Vertices[4], Vertices[0], Vertices[5]), new Triangle(Vertices[0], Vertices[1], Vertices[5]));   // right
            Face4.Create(this, new Triangle(Vertices[7], Vertices[3], Vertices[6]), new Triangle(Vertices[3], Vertices[2], Vertices[6]));   // left

            // Create anchor point
            Anchor = Anchor.Create(this);
            Selection.activeGameObject = Anchor.gameObject;

            // Create mesh preview tool
            MeshPreview = MeshPreview.Create(this);
        }

        protected void SetupMeshEditing(GeometryData data)
        {
            foreach (var vertex in data.Vertices)
            {
                Vertex.Create(this, vertex);
            }

            foreach (var edge in data.Edges)
            {
                var vertex1 = GetVertexAtPosition(edge.Vertex1);
                var vertex2 = GetVertexAtPosition(edge.Vertex2);

                Edge.Create(this, vertex1, vertex2);
            }

            foreach (var face in data.Faces)
            {
                if (face.Face4)
                {
                    var tri1 = new Triangle(GetVertexAtPosition(face.Triangles[0].Vertices[0]), GetVertexAtPosition(face.Triangles[0].Vertices[1]), GetVertexAtPosition(face.Triangles[0].Vertices[2]));
                    var tri2 = new Triangle(GetVertexAtPosition(face.Triangles[1].Vertices[0]), GetVertexAtPosition(face.Triangles[1].Vertices[1]), GetVertexAtPosition(face.Triangles[1].Vertices[2]));
                    Face4.Create(this, tri1, tri2);
                }
                else
                {
                    var tri = new Triangle(GetVertexAtPosition(face.Triangles[0].Vertices[0]), GetVertexAtPosition(face.Triangles[0].Vertices[1]), GetVertexAtPosition(face.Triangles[0].Vertices[2]));
                    Face3.Create(this, tri);
                }
            }

            Anchor = Anchor.Create(this);
            Selection.activeGameObject = Anchor.gameObject;

            MeshPreview = MeshPreview.Create(this);
        }

        public void RebuildGeometry()
        {
            var oldGeometry = EditingGeometry;
            BuildGeometry();
            DestroyImmediate(oldGeometry.gameObject);
        }

        public void BuildGeometry()
        {
            var parentGameObject = new GameObject("Geometry");
            parentGameObject.transform.position = Anchor.transform.position;

            var built = BuiltGeometry.Create(parentGameObject, this);
            var meshes = GenerateQuads();

            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                var meshGameObject = new GameObject("Face", typeof(MeshFilter), typeof(MeshRenderer));

                meshGameObject.transform.position = Anchor.transform.position;
                meshGameObject.transform.SetParent(parentGameObject.transform, true);

                meshGameObject.GetComponent<MeshFilter>().mesh = mesh;

                if (EditType == GeometryEditType.New)
                {
                    meshGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Settings.BaseMaterial)
                    {
                        name = $"{Settings.SelectedTexture.name} Material",
                        mainTexture = Settings.SelectedTexture
                    };
                }
                else
                {
                    meshGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(EditingGeometry.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial);
                }

                meshGameObject.AddComponent<EditableTexture>();
            }

            Settings.LastBuiltGeometryData = new GeometryData(this);

            Create(built);
        }
        
        public Mesh GenerateMesh()
        {
            var mesh = new Mesh();

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach (var vertex in Vertices)
            {
                vertices.Add(vertex.transform.position - vertex.GeometryEditor.Anchor.transform.position);
            }

            foreach (var face in Faces)
            {
                if (face is Face4 f4)
                {
                    triangles.AddRange(new[]
                    {
                        f4.Triangle1.Vertices[0].Index,
                        f4.Triangle1.Vertices[1].Index,
                        f4.Triangle1.Vertices[2].Index,

                        f4.Triangle2.Vertices[0].Index,
                        f4.Triangle2.Vertices[1].Index,
                        f4.Triangle2.Vertices[2].Index
                    });
                }
                else if (face is Face3 f3)
                {
                    triangles.AddRange(new[]
                    {
                        f3.Triangle.Vertices[0].Index,
                        f3.Triangle.Vertices[1].Index,
                        f3.Triangle.Vertices[2].Index
                    });
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }

        public List<Mesh> GenerateQuads()
        {
            var meshes = new List<Mesh>();
            foreach (var face in Faces)
            {
                var mesh = new Mesh();
                var triangles = new List<int>();
                var vertices = new List<Vector3>();
                var vertexList = face.Vertices.ToList();

                foreach (var vertex in face.Vertices)
                {
                    vertices.Add(vertex.transform.position - vertex.GeometryEditor.Anchor.transform.position);
                }

                if (face is Face4 f4)
                {
                    triangles.AddRange(new[]
                    {
                        vertexList.IndexOf(f4.Triangle1.Vertices[0]),
                        vertexList.IndexOf(f4.Triangle1.Vertices[1]),
                        vertexList.IndexOf(f4.Triangle1.Vertices[2]),

                        vertexList.IndexOf(f4.Triangle2.Vertices[0]),
                        vertexList.IndexOf(f4.Triangle2.Vertices[1]),
                        vertexList.IndexOf(f4.Triangle2.Vertices[2]),
                    });
                }
                else if (face is Face3 f3)
                {
                    triangles.AddRange(new[]
                    {
                        vertexList.IndexOf(f3.Triangle.Vertices[0]),
                        vertexList.IndexOf(f3.Triangle.Vertices[1]),
                        vertexList.IndexOf(f3.Triangle.Vertices[2]),
                    });
                }

                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();

                mesh.Optimize();
                mesh.RecalculateNormals();

                mesh.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

                meshes.Add(mesh);
            }

            return meshes;
        }

        public Vertex GetVertexAtPosition(Vector3 position)
        {
            foreach (var vertex in Vertices)
            {
                if (vertex.transform.position == position)
                {
                    return vertex;
                }
            }

            return null;
        }

        public static GeometryEditor Create()
        {
            var editor = new GameObject("Geometry Editor", typeof(GeometryEditor)).GetComponent<GeometryEditor>();

            editor.EditType = GeometryEditType.New;
            editor.SetupCubeEditing();

            return editor;
        }

        public static GeometryEditor Create(GeometryData data)
        {
            var editor = new GameObject("Geometry Editor", typeof(GeometryEditor)).GetComponent<GeometryEditor>();

            editor.SetupMeshEditing(data);

            return editor;
        }

        public static GeometryEditor Create(BuiltGeometry geometry)
        {
            var isRebuild = false;
            GeometryEditor editor;
            if (Current == null)
            {
                editor = new GameObject("Geometry Editor", typeof(GeometryEditor)).GetComponent<GeometryEditor>();
            }
            else
            {
                editor = Current;
                isRebuild = true;
            }

            editor.EditType = GeometryEditType.Existing;
            editor.EditingGeometry = geometry;

            if (!isRebuild)
            {
                editor.SetupMeshEditing(geometry.GeometryData);
            }

            return editor;
        }
    }
}
#endif
