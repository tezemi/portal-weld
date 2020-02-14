using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace PortalWeld.TextureTool
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class EditableTexture : MonoBehaviour
    {
        #if UNITY_EDITOR
        [HideInInspector]
        public Material Material;
        protected MeshFilter MeshFilter { get; set; }
        protected const string MainTexProperty = "_MainTex";
        protected const string RotationProperty = "_Rotation";
        private Vector2 _tiling = Vector2.one;

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
                    if (Material.GetFloat(RotationProperty) != value)
                    {
                        EditorSceneManager.MarkSceneDirty(gameObject.scene);
                    }

                    Material.SetFloat(RotationProperty, value);
                }
            }
        }

        public Vector2 Tiling
        {
            get
            {
                return _tiling;
            }
            set
            {
                if (Material.GetTextureScale(MainTexProperty) != value)
                {
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }

                float x;
                if (MeshFilter.sharedMesh.bounds.size.x > MeshFilter.sharedMesh.bounds.size.z)
                {
                    x = MeshFilter.sharedMesh.bounds.size.x;
                }
                else
                {
                    x = MeshFilter.sharedMesh.bounds.size.z;
                }

                float y;
                if (MeshFilter.sharedMesh.bounds.size.y > MeshFilter.sharedMesh.bounds.size.z || MeshFilter.sharedMesh.bounds.size.y > MeshFilter.sharedMesh.bounds.size.x)
                {
                    y = MeshFilter.sharedMesh.bounds.size.y;
                }
                else if (MeshFilter.sharedMesh.bounds.size.x > MeshFilter.sharedMesh.bounds.size.z)
                {
                    y = MeshFilter.sharedMesh.bounds.size.z;
                }
                else
                {
                    y = MeshFilter.sharedMesh.bounds.size.x;
                }

                x /= Settings.TextureScale;
                y /= Settings.TextureScale;

                Material.SetTextureScale(MainTexProperty, value * new Vector2(x, y));

                _tiling = value;
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
                if (Material.GetTextureOffset(MainTexProperty) != value)
                {
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }

                Material.SetTextureOffset(MainTexProperty, value);
            }
        }

        protected virtual void Awake()
        {
            MeshFilter = GetComponent<MeshFilter>();
            Material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        public void GroupWithTexture()
        {
            const string geometryRootName = "Geometry";

            // Find the root of all geometry, and the root for specific textures
            GameObject root = null;
            foreach (var obj in gameObject.scene.GetRootGameObjects())
            {
                if (obj.name == geometryRootName)
                {
                    root = obj;
                    break;
                }
            }

            if (root == null)
            {
                root = new GameObject(geometryRootName);
                SceneManager.MoveGameObjectToScene(root, gameObject.scene);
            }

            GameObject textureRoot = null;
            foreach (Transform child in root.transform)
            {
                if (child.name == Material.mainTexture.name)
                {
                    textureRoot = child.gameObject;
                    break;
                }
            }

            if (textureRoot == null)
            {
                textureRoot = new GameObject(Material.mainTexture.name);
                SceneManager.MoveGameObjectToScene(textureRoot, gameObject.scene);
            }

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
                    if (selectedGeometry.GeometryEditor != null)
                    {
                        selectedGeometry.GeometryEditor.gameObject.SetActive(true);
                    }
                }
            };
        }
        #endif
    }
}
