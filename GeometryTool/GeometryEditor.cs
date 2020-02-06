#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using PortalWeld.TextureTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    /// <summary>
    /// Component used to control portal weld's geometry editor. This 
    /// component is responsible for keeping track of all elements of 
    /// the geometry editor, including vertices, edges, and faces, as
    /// well as building the geometry itself.
    /// </summary>
    [ExecuteAlways]
    public class GeometryEditor : MonoBehaviour
    {
        /// <summary>
        /// The currently active geometry editor. There should only be one 
        /// geometry editor at a given time.
        /// </summary>
        public static GeometryEditor Current { get; private set; }
        /// <summary>
        /// Gets set to true if this geometry editor is being destroyed. This 
        /// is checked by other elements of the geometry editor to see if they 
        /// are being destroyed individually or as part of the entire object.
        /// </summary>
        [HideInInspector]
        public bool IsBeingDestroyed;
        /// <summary>
        /// In edit mode, the editor is being used to reshape already built 
        /// geometry, as opposed to create new geometry.
        /// </summary>
        [HideInInspector]
        public bool EditMode;
        /// <summary>
        /// The anchor is a component that remains at the center of the 
        /// geometry and is used to move the entire editor and all of its 
        /// elements.
        /// </summary>
        [HideInInspector]
        public Anchor Anchor;
        /// <summary>
        /// A component used to preview what the current geometry will look 
        /// like when it's built.
        /// </summary>
        [HideInInspector]
        public MeshPreview MeshPreview;
        /// <summary>
        /// Built geometry that is currently being edited. This will be null 
        /// if the editor is being used to create new geometry.
        /// </summary>
        [HideInInspector]
        public Geometry GeometryBeingEdited;
        /// <summary>
        /// All of the faces associated with this geometry.
        /// </summary>
        [HideInInspector]
        public List<Face> Faces = new List<Face>();
        /// <summary>
        /// All of the edges associated with this geometry.
        /// </summary>
        [HideInInspector]
        public List<Edge> Edges = new List<Edge>();
        /// <summary>
        /// All of the vertices associated with this geometry.
        /// </summary>
        [HideInInspector]
        public List<Vertex> Vertices = new List<Vertex>();
        /// <summary>
        /// All of the 2D vertices assosciated with this geometry.
        /// </summary>
        [HideInInspector]
        public List<Vertex2D> Vertices2D = new List<Vertex2D>();
        /// <summary>
        /// A list containing every element part of this editor.
        /// </summary>
        [HideInInspector]
        public List<GeometryEditorElement> Elements = new List<GeometryEditorElement>();

        protected virtual void Awake()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;   // The only game object exposed to the user is the anchor
        }

        protected virtual void OnEnable()
        {
            if (Current != null && Current != this)
            {
                if (!Current.EditMode)
                {
                    DestroyImmediate(Current.Anchor.gameObject);
                }
                else
                {
                    Current.gameObject.SetActive(false);
                }
            }

            Current = this;

            foreach (var element in Elements)
            {
                element.gameObject.SetActive(true);
                element.enabled = true;
            }

            if (Anchor != null)
            {
                Anchor.gameObject.hideFlags = HideFlags.None;
                Anchor.gameObject.transform.SetAsLastSibling();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var element in Elements)
            {
                element.gameObject.SetActive(false);
                element.enabled = false;
            }

            Anchor.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        protected virtual void OnDestroy()
        {
            IsBeingDestroyed = true;

            // Gathers all elements of this editor and destroys them
            var cleanup = new List<GameObject>();
            foreach (var element in Elements)
            {
                cleanup.Add(element.gameObject);
            }

            foreach (var clean in cleanup)
            {
                if (clean != Anchor.gameObject)
                {
                    DestroyImmediate(clean);
                }
            }

            DestroyImmediate(MeshPreview.gameObject);

            if (EditMode && GeometryBeingEdited != null)
            {
                DestroyImmediate(GeometryBeingEdited.gameObject);
            }
        }

        /// <summary>
        /// Sets up this editor using cube shaped geometry.
        /// </summary>
        /// <param name="position">The position to create the cube.</param>
        protected void SetupCubeEditing(Vector3 position)
        {
            const float cubeSize = 3f;

            // GeometryEditor always starts as a cube
            // This creates eight vertices in a cube shape
            Vertex.Create(this, position + new Vector3(cubeSize, cubeSize, cubeSize));
            Vertex.Create(this, position + new Vector3(-cubeSize, cubeSize, cubeSize));
            Vertex.Create(this, position + new Vector3(cubeSize, cubeSize, -cubeSize));
            Vertex.Create(this, position + new Vector3(-cubeSize, cubeSize, -cubeSize));

            Vertex.Create(this, position + new Vector3(cubeSize, -cubeSize, cubeSize));
            Vertex.Create(this, position + new Vector3(-cubeSize, -cubeSize, cubeSize));
            Vertex.Create(this, position + new Vector3(cubeSize, -cubeSize, -cubeSize));
            Vertex.Create(this, position + new Vector3(-cubeSize, -cubeSize, -cubeSize));

            // Create the edges of the cube
            Edge.Create(this, Vertices[0], Vertices[1]);
            Edge.Create(this, Vertices[0], Vertices[2]);
            Edge.Create(this, Vertices[1], Vertices[3]);
            Edge.Create(this, Vertices[2], Vertices[3]);

            Edge.Create(this, Vertices[4], Vertices[5]);
            Edge.Create(this, Vertices[4], Vertices[6]);
            Edge.Create(this, Vertices[5], Vertices[7]);
            Edge.Create(this, Vertices[6], Vertices[7]);

            Edge.Create(this, Vertices[0], Vertices[4]);
            Edge.Create(this, Vertices[1], Vertices[5]);
            Edge.Create(this, Vertices[2], Vertices[6]);
            Edge.Create(this, Vertices[3], Vertices[7]);

            // Create the face objects
            // Order of vertices within triangles is important, defines normals, UVs
            var top = Face4.Create(this, new Triangle(Vertices[0], Vertices[2], Vertices[1]), new Triangle(Vertices[2], Vertices[3], Vertices[1]));     // top
            var bottom = Face4.Create(this, new Triangle(Vertices[6], Vertices[4], Vertices[7]), new Triangle(Vertices[4], Vertices[5], Vertices[7]));  // bottom

            var front = Face4.Create(this, new Triangle(Vertices[6], Vertices[2], Vertices[4]), new Triangle(Vertices[2], Vertices[0], Vertices[4]));   // front
            var back = Face4.Create(this, new Triangle(Vertices[5], Vertices[1], Vertices[7]), new Triangle(Vertices[1], Vertices[3], Vertices[7]));    // back

            var left = Face4.Create(this, new Triangle(Vertices[4], Vertices[0], Vertices[5]), new Triangle(Vertices[0], Vertices[1], Vertices[5]));    // right
            var right = Face4.Create(this, new Triangle(Vertices[7], Vertices[3], Vertices[6]), new Triangle(Vertices[3], Vertices[2], Vertices[6]));   // left

            // Create elements used for 2D editing
            const float offset = 1f;
            Vertex2D.Create(this, front, ViewSide.Top | ViewSide.Front, new Vector3(offset, 0f, 0f));
            Vertex2D.Create(this, back, ViewSide.Top | ViewSide.Front, new Vector3(-offset, 0f, 0f));

            Vertex2D.Create(this, left, ViewSide.Top | ViewSide.Side, new Vector3(0f, 0f, offset));
            Vertex2D.Create(this, right, ViewSide.Top | ViewSide.Side, new Vector3(0f, 0f, -offset));

            Vertex2D.Create(this, top, ViewSide.Front | ViewSide.Side, new Vector3(0f, offset, 0f));
            Vertex2D.Create(this, bottom, ViewSide.Front | ViewSide.Side, new Vector3(0f, -offset, 0f));
            
            //Vertex2D.Create(this, dbottom, dright, ViewSide.Top);

            // Create anchor point
            Anchor = Anchor.Create(this);
            Selection.activeGameObject = Anchor.gameObject;

            // Create mesh preview tool
            MeshPreview = MeshPreview.Create(this);
        }

        /// <summary>
        /// If in edit mode, this will destroy the current geometry and 
        /// rebuild it from scratch.
        /// </summary>
        public void RebuildGeometry()
        {
            var oldGeometry = GeometryBeingEdited;
            BuildGeometry();
            DestroyImmediate(oldGeometry.gameObject);
        }

        /// <summary>
        /// Builds geometry using the configured vertices, faces, and edges.
        /// </summary>
        public void BuildGeometry()
        {
            var parentGameObject = new GameObject();                                            // One game object will serve as the parent
            parentGameObject.transform.position = Anchor.transform.position;

            var built = Geometry.Create(parentGameObject, this);                                // This component is used to keep track of geometry data (verts, faces, tris)
            var meshes = GenerateQuads();                                                       // This creates a quad for every face

            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];   // Assign every quad to a new object
                var meshGameObject = new GameObject("Face", typeof(MeshFilter), typeof(MeshRenderer));

                meshGameObject.transform.position = Anchor.transform.position;
                meshGameObject.transform.SetParent(parentGameObject.transform, true);

                meshGameObject.GetComponent<MeshFilter>().mesh = mesh;

                if (!EditMode)  // Assign the default texture when not in edit mode
                {
                    meshGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Settings.BaseMaterial)
                    {
                        name = $"{Settings.SelectedTexture.name} Material",
                        mainTexture = Settings.SelectedTexture
                    };
                }
                else            // Assign the texture on the previous face if in edit mode
                {
                    meshGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(GeometryBeingEdited.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial);
                }

                meshGameObject.layer = Settings.GeometryLayerMask;
                if (Settings.IsSolid)
                {
                    meshGameObject.AddComponent<BoxCollider>();
                }

                meshGameObject.isStatic = Settings.IsStatic;

                var editableTexture = meshGameObject.AddComponent<EditableTexture>();
                editableTexture.GroupWithTexture();

                // Call some callbacks
                PortalWeldCallbacks.TextureApplied?.Invoke(meshGameObject.GetComponent<MeshRenderer>(), meshGameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture);
                PortalWeldCallbacks.MeshBuilt?.Invoke(meshGameObject.GetComponent<MeshFilter>());
            }

            parentGameObject.isStatic = Settings.IsStatic;

            PortalWeldCallbacks.GeometryBuilt?.Invoke(parentGameObject);

            Settings.LastBuiltGeometryPosition = Anchor.transform.position;

            // This geometry editor will now edit the built geometry
            if (!EditMode)
            {
                EditMode = true;
                Selection.activeGameObject = Anchor.gameObject;
            }

            GeometryBeingEdited = built;
        }

        /// <summary>
        /// Generates a mesh that looks like the defined geometry. Does not 
        /// assign textures, for that, use the BuildGeometry method.
        /// </summary>
        /// <returns>The generated mesh.</returns>
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

        /// <summary>
        /// Generates a quad mesh for each face of the geometry.
        /// </summary>
        /// <returns>A list of all generated quads.</returns>
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

        /// <summary>
        /// Gets the vertex component at the specified position.
        /// </summary>
        /// <param name="position">The position to get the vertex at.</param>
        /// <returns>The vertex, if one exists at that position.</returns>
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

        /// <summary>
        /// Creates a new geometry editor at the specified position using the 
        /// defined shape.
        /// </summary>
        /// <param name="shape">The shape to setup that the new geometry editor 
        /// will take.</param>
        /// <param name="position">The position to create the new geometry at.</param>
        /// <returns>The newly created geometry editor.</returns>
        public static GeometryEditor Create(Shape shape, Vector3 position)
        {
            var editor = new GameObject("Geometry Editor", typeof(GeometryEditor)).GetComponent<GeometryEditor>();

            if (shape == Shape.Cube)
            {
                editor.SetupCubeEditing(position);
            }

            return editor;
        }
    }
}
#endif
