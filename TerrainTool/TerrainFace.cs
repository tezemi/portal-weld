using UnityEngine;

namespace PortalWeld.TerrainTool
{
    #if UNITY_EDITOR
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    public class TerrainFace : MonoBehaviour
    {
        [HideInInspector]
        public MeshFilter MeshFilter;

        public void Rebuild()
        {
            var array = new Vector3[MeshFilter.sharedMesh.vertices.Length];
            foreach (var vertex in GetComponentsInChildren<TerrainVertex>())
            {
                array[vertex.IndexInVertexArray] = vertex.transform.position - transform.parent.position;
            }

            MeshFilter.sharedMesh.vertices = array;
            MeshFilter.sharedMesh.RecalculateBounds();
            MeshFilter.sharedMesh.RecalculateNormals();
            MeshFilter.sharedMesh.RecalculateTangents();
        }

        public static TerrainFace Create(GameObject obj, int power)
        {
            var face = obj.AddComponent<TerrainFace>();

            face.MeshFilter = obj.GetComponent<MeshFilter>();

            MeshHelper.Subdivide(face.MeshFilter.sharedMesh, power);

            for (var i = 0; i < face.MeshFilter.sharedMesh.vertices.Length; i++)
            {
                TerrainVertex.Create(i, face);
            }

            return face;
        }
    }
    #endif
}

