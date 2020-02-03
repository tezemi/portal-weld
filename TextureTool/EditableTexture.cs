#if UNITY_EDITOR
using PortalWeld.GeometryTool;
using UnityEditor;
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

        public void GroupWithTexture()
        {
            const string geometryRootName = "Geometry";

            // Find the root of all geometry, and the root for specific textures
            var root = GameObject.Find(geometryRootName) ?? new GameObject(geometryRootName);
            var textureRoot = GameObject.Find(Material.mainTexture.name) ?? new GameObject(Material.mainTexture.name);

            textureRoot.transform.SetParent(root.transform);

            transform.parent.SetParent(textureRoot.transform);
            transform.parent.name = $"Geometry {transform.parent.GetSiblingIndex()}";
        }

        static EditableTexture()
        {
            Selection.selectionChanged += () =>
            {
                if (Utilities.IsSelected<EditableTexture>())
                {
                    var selectedGeometry = Utilities.GetFromSelectionParent<Geometry>();
                    selectedGeometry.GeometryEditor.gameObject.SetActive(true);
                }
            };
        }
    }
}
#endif
