#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.TerrainTool
{
    [ExecuteAlways]
    public class TerrainVertex : MonoBehaviour
    {
        [HideInInspector]
        public int IndexInVertexArray;
        [HideInInspector]
        public Vector3 PositionLastFrame;
        [HideInInspector]
        public TerrainFace Face;
        
        protected virtual void Update()
        {
            if (transform.position !=  PositionLastFrame && Utilities.IsSelected(this))
            {
                if (TerrainSettings.CurrentTerrainTool == TerrainTools.RaiseLower)
                {
                    foreach (var vertex in FindObjectsOfType<TerrainVertex>())
                    {
                        var distance = Vector3.Distance(vertex.transform.position, transform.position);
                        if (vertex != this && distance < TerrainSettings.TerrainToolSize)
                        {
                            vertex.transform.position += (transform.position - PositionLastFrame) / distance * 2f;
                        }
                    }
                }

                GetComponentInParent<TerrainFace>().Rebuild();
            }

            PositionLastFrame = transform.position;
        }

        protected virtual void OnDrawGizmos()
        {
            if (TerrainWindow.Current != null)
            {
                Gizmos.color = Utilities.IsSelected(this) ? Color.blue : Color.red;
                Gizmos.DrawSphere(transform.position, 0.2f);

                if (TerrainSettings.CurrentTerrainTool == TerrainTools.RaiseLower && Utilities.IsSelected(this))
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(transform.position, TerrainSettings.TerrainToolSize);
                }
            }
        }

        public static TerrainVertex Create(int index, TerrainFace face)
        {
            var vertex = new GameObject("Vertex", typeof(TerrainVertex)).GetComponent<TerrainVertex>();

            vertex.transform.position = face.transform.position + face.MeshFilter.sharedMesh.vertices[index];
            vertex.transform.SetParent(face.transform, true);
            vertex.PositionLastFrame = vertex.transform.position;
            vertex.IndexInVertexArray = index;
            vertex.Face = face;

            return vertex;
        }
    }
}
#endif
