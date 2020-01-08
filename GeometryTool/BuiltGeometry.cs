#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class BuiltGeometry : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 GlobalPosition;
        [HideInInspector]
        public FaceData[] Faces;
        [HideInInspector]
        public EdgeData[] Edges;
        [HideInInspector]
        public Vector3[] Vertices;

        public static BuiltGeometry Create(GameObject geometryGameObject, GeometryEditor editor)
        {
            var builtGeometry = geometryGameObject.AddComponent<BuiltGeometry>();

            builtGeometry.GlobalPosition = editor.Anchor.Center;

            builtGeometry.Vertices = new Vector3[editor.Vertices.Count];
            for (var i = 0; i < builtGeometry.Vertices.Length; i++)
            {
                builtGeometry.Vertices[i] = editor.Vertices[i].transform.position;
            }

            builtGeometry.Edges = new EdgeData[editor.Edges.Count];
            for (var i = 0; i < builtGeometry.Edges.Length; i++)
            {
                builtGeometry.Edges[i] = new EdgeData(editor.Edges[i].Vertex1.transform.position, editor.Edges[i].Vertex2.transform.position);
            }

            builtGeometry.Faces = new FaceData[editor.Faces.Count];
            for (var i = 0; i < builtGeometry.Faces.Length; i++)
            {
                var face = editor.Faces[i];

                var vertices = new Vector3[face.Vertices.Length];
                for (var j = 0; j < vertices.Length; j++)
                {
                    vertices[j] = face.Vertices[j].transform.position;
                }

                var triangles = new TriangleData[0];
                if (face is Face3 f3)
                {
                    triangles = new[] { new TriangleData(new []
                    {
                        f3.Triangle.Vertices[0].transform.position,
                        f3.Triangle.Vertices[1].transform.position,
                        f3.Triangle.Vertices[2].transform.position,
                    }) };
                }
                else if (face is Face4 f4)
                {
                    triangles = new[] { new TriangleData(new []
                    {
                        f4.Triangle1.Vertices[0].transform.position,
                        f4.Triangle1.Vertices[1].transform.position,
                        f4.Triangle1.Vertices[2].transform.position
                    }), new TriangleData(new []
                    {
                        f4.Triangle2.Vertices[0].transform.position,
                        f4.Triangle2.Vertices[1].transform.position,
                        f4.Triangle2.Vertices[2].transform.position
                    })
                    };
                }
                
                builtGeometry.Faces[i] = new FaceData(face is Face4, vertices, triangles);
            }

            return builtGeometry;
        }

        [Serializable]
        public struct EdgeData
        {
            public Vector3 Vertex1;
            public Vector3 Vertex2;

            public EdgeData(Vector3 vertex1, Vector3 vertex2)
            {
                Vertex1 = vertex1;
                Vertex2 = vertex2;
            }
        }
        
        [Serializable]
        public struct FaceData
        {
            public bool Face4;
            public Vector3[] Vertices;
            public TriangleData[] Triangles;

            public FaceData(bool face4, Vector3[] vertices, TriangleData[] triangles)
            {
                Face4 = face4;
                Vertices = vertices;
                Triangles = triangles;
            }
        }

        [Serializable]
        public struct TriangleData
        {
            public Vector3[] Vertices;

            public TriangleData(Vector3[] vertices)
            {
                Vertices = vertices;
            }
        }
    }
}
#endif
