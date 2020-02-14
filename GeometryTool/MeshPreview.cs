using UnityEngine;

namespace PortalWeld.GeometryTool
{
    public class MeshPreview : MonoBehaviour
    {
        #if UNITY_EDITOR
        [HideInInspector]
        public GeometryEditor GeometryEditor;
        [HideInInspector]
        public Mesh PreviewMesh;
        protected readonly Color PreviewMeshColor = new Color(1f, 0.15f, 0.45f, 0.5f);

        protected virtual void OnDrawGizmos()
        {
            if (Settings.ShowMeshPreview)
            {
                if (PreviewMesh == null)
                {
                    UpdatePreview();
                }

                Gizmos.color = PreviewMeshColor;
                Gizmos.DrawMesh(PreviewMesh, GeometryEditor.Anchor.transform.position);
            }
        }

        public void UpdatePreview()
        {
            PreviewMesh = GeometryEditor.GenerateMesh();
        }

        public static MeshPreview Create(GeometryEditor geometryEditor)
        {
            var meshPreview = new GameObject("Mesh Preview", typeof(MeshPreview)).GetComponent<MeshPreview>();

            meshPreview.GeometryEditor = geometryEditor;
            meshPreview.transform.SetParent(geometryEditor.transform);
            
            return meshPreview;
        }
        #endif
    }
}

