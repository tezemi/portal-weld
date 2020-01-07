#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class Face3 : Face
    {
        [HideInInspector]
        public Triangle Triangle;

        public static Face3 Create(GeometryEditor geometryEditor, Triangle triangle)
        {
            var b = CreateBaseFace<Face3>(geometryEditor, triangle.Vertices[0], triangle.Vertices[1], triangle.Vertices[2]);
            b.Triangle = triangle;

            return b;
        }
    }
}
#endif
