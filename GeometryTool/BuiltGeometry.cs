#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class BuiltGeometry : MonoBehaviour
    {
        [HideInInspector]
        public GeometryData GeometryData;

        public static BuiltGeometry Create(GameObject geometryGameObject, GeometryEditor editor)
        {
            var builtGeometry = geometryGameObject.AddComponent<BuiltGeometry>();
            builtGeometry.GeometryData = new GeometryData(editor);

            return builtGeometry;
        }
    }
}
#endif
