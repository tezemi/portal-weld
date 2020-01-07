#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class Face4 : Face
    {
        [HideInInspector]
        public Triangle Triangle1;
        [HideInInspector]
        public Triangle Triangle2;

        public static Face4 Create(GeometryEditor geometryEditor, Triangle triangle1, Triangle triangle2)
        {
            var uniqueVertices = new List<Vertex>();
            foreach (var vertex in triangle1.Vertices)
            {
                if (!uniqueVertices.Contains(vertex))
                {
                    uniqueVertices.Add(vertex);
                }
            }

            foreach (var vertex in triangle2.Vertices)
            {
                if (!uniqueVertices.Contains(vertex))
                {
                    uniqueVertices.Add(vertex);
                }
            }

            var b = CreateBaseFace<Face4>(geometryEditor, uniqueVertices.ToArray());
            b.Triangle1 = triangle1;
            b.Triangle2 = triangle2;

            return b;
        }
    }
}
#endif
