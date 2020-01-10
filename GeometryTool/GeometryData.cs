#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    [Serializable]
    public struct GeometryData
    {
        public Vector3 AnchorPosition;
        public FaceData[] Faces;
        public EdgeData[] Edges;
        public Vector3[] Vertices;

        public GeometryData(Vector3 anchorPosition, FaceData[] faces, EdgeData[] edges, Vector3[] vertices)
        {
            AnchorPosition = anchorPosition;
            Faces = faces;
            Edges = edges;
            Vertices = vertices;
        }

        public GeometryData(GeometryEditor editor)
        {
            AnchorPosition = editor.Anchor.transform.position;

            Vertices = new Vector3[editor.Vertices.Count];
            for (var i = 0; i <Vertices.Length; i++)
            {
               Vertices[i] = editor.Vertices[i].transform.position;
            }

            Edges = new EdgeData[editor.Edges.Count];
            for (var i = 0; i <Edges.Length; i++)
            {
               Edges[i] = new EdgeData(editor.Edges[i].Vertex1.transform.position, editor.Edges[i].Vertex2.transform.position);
            }

            Faces = new FaceData[editor.Faces.Count];
            for (var i = 0; i <Faces.Length; i++)
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

                Faces[i] = new FaceData(face is Face4, vertices, triangles);
            }
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
