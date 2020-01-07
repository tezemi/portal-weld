#if UNITY_EDITOR
using System;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    [Serializable]
    public class Triangle
    {
        [HideInInspector]
        public Mesh DebugMesh;
        [HideInInspector]
        public Vertex[] Vertices = new Vertex[3];
        
        public void Rotate()
        {
            var temp1 = Vertices[0];
            var temp2 = Vertices[1];
            var temp3 = Vertices[2];

            Vertices[0] = temp3;
            Vertices[1] = temp2;
            Vertices[2] = temp1;
        }

        public Triangle(Vertex vertex1, Vertex vertex2, Vertex vertex3)
        {
            Vertices[0] = vertex1;
            Vertices[1] = vertex2;
            Vertices[2] = vertex3;
        }
    }
}
#endif
