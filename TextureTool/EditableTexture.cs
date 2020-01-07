#if UNITY_EDITOR
using UnityEngine;

namespace PortalWeld.TextureTool
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class EditableTexture : MonoBehaviour
    {
        [HideInInspector]
        public Material Material;
        protected const string MainTexProperty = "_MainTex";
        protected const string RotationProperty = "_Rotation";
        
        public float Rotation
        {
            get
            {
                return !Material.HasProperty(RotationProperty) ? 0f : Material.GetFloat(RotationProperty);
            }
            set
            {
                if (Material.HasProperty(RotationProperty))
                {
                    Material.SetFloat(RotationProperty, value);
                }
            }
        }

        public Vector2 Tiling
        {
            get
            {
                return Material.GetTextureScale(MainTexProperty);
            }
            set
            {
                Material.SetTextureScale(MainTexProperty, value);
            }
        }

        public Vector2 Offset
        {
            get
            {
                return Material.GetTextureOffset(MainTexProperty);
            }
            set
            {
                Material.SetTextureOffset(MainTexProperty, value);
            }
        }

        protected virtual void Awake()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            Material = GetComponent<MeshRenderer>().sharedMaterial;
            Tiling = new Vector2
            (
                mesh.bounds.size.x == 0f ? mesh.bounds.size.z / 2f : mesh.bounds.size.x / 2f,
                mesh.bounds.size.y == 0f ? mesh.bounds.size.z / 2f : mesh.bounds.size.y / 2f
            );
        }
    }
}
#endif
