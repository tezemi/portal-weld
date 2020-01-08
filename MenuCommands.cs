#if UNITY_EDITOR
using System.Linq;
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PortalWeld
{
    /// <summary>
    /// Contains all of the menu commands for portal weld.
    /// </summary>
    public static class MenuCommands
    {
        private const string ShowMeshPreviewPath = "Portal Weld/Geometry/Show Mesh Preview";
        private const string ShowDynamicGizmosPath = "Portal Weld/Geometry/Show Dynamic Gizmos";
        private const string SnapToGridPath = "Portal Weld/Geometry/Snap to Grid";

        static MenuCommands()
        {
            Menu.SetChecked(ShowMeshPreviewPath, Settings.ShowMeshPreview);
            Menu.SetChecked(ShowDynamicGizmosPath, Settings.ShowDynamicGizmos);
            Menu.SetChecked(SnapToGridPath, Settings.SnapToGrid);
        }
        
        [MenuItem(ShowMeshPreviewPath)]
        private static void ShowMeshPreview()
        {
            Settings.ShowMeshPreview = !Settings.ShowMeshPreview;
            Menu.SetChecked(ShowMeshPreviewPath, Settings.ShowMeshPreview);
        }

        [MenuItem(ShowDynamicGizmosPath)]
        private static void ShowDynamicGizmos()
        {
            Settings.ShowDynamicGizmos = !Settings.ShowDynamicGizmos;
            Menu.SetChecked(ShowDynamicGizmosPath, Settings.ShowDynamicGizmos);
        }

        [MenuItem(SnapToGridPath)]
        private static void SnapToGrid()
        {
            Settings.SnapToGrid = !Settings.SnapToGrid;
            Menu.SetChecked(SnapToGridPath, Settings.SnapToGrid);
        }

        [MenuItem("Portal Weld/Geometry/Edit Geometry #e")]
        private static void EditGeometry()
        {
            if (Utilities.SelectionHas<BuiltGeometry>())
            {
                var builtGeometry = Utilities.GetFromSelection<BuiltGeometry>();
                GeometryEditor.Create(builtGeometry);
            }
        }

        [MenuItem("Portal Weld/Geometry/Bifurcate Edge #b")]
        private static void BifurcateEdge()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject && gameObject.HasComponent<Edge>())
                {
                    gameObject.GetComponent<Edge>().Bifurcate();
                }
            }

            GeometryEditor.Current.MeshPreview.UpdatePreview();
        }

        [MenuItem("Portal Weld/Geometry/Flip Triangle(s) #f")]
        private static void FlipTriangles()
        {
            if (Selection.activeGameObject.HasComponent<Face>())
            {
                var face = Selection.activeGameObject.GetComponent<Face>();
                if (face is Face3 f3)
                {
                    f3.Triangle.Rotate();
                }
                else if (face is Face4 f4)
                {
                    f4.Triangle1.Rotate();
                    f4.Triangle2.Rotate();
                }
            }

            GeometryEditor.Current.MeshPreview.UpdatePreview();
        }

        [MenuItem("Portal Weld/Geometry/Combine Faces #c")]
        private static void CombineFaces()
        {
            var selection = Utilities.Get2FromSelection<Face3>();
            var face1 = selection.Item1;
            var face2 = selection.Item2;
            if (face1 != null && face2 != null)
            {
                Vertex vertex4 = null;
                foreach (var vertex in face2.Vertices)
                {
                    if (vertex != face1.Vertices[0] && vertex != face1.Vertices[1] && vertex != face1.Vertices[2])
                    {
                        vertex4 = vertex;
                        break;
                    }
                }

                Face4.Create(face1.GeometryEditor, new Triangle(face1.Vertices[0], face1.Vertices[1], face1.Vertices[2]), new Triangle(vertex4, face1.Vertices[2], face1.Vertices[1]));
                Object.DestroyImmediate(face1);
                Object.DestroyImmediate(face2);
            }
        }

        [MenuItem("Portal Weld/Geometry/Create Face %#c")]
        private static void CreateFace()
        {
            if (Selection.objects.Length == 3)
            {
                var vertices = Utilities.Get3FromSelection<Vertex>();
                if (vertices.Item1 != null && vertices.Item2 != null && vertices.Item3 != null)
                {
                    var vertexArray = new[] { vertices.Item1, vertices.Item2, vertices.Item3 };
                    Utilities.ConnectUnconnectedVertices(vertexArray);
                    vertexArray = vertexArray.OrderBy(v => v.Index).ToArray();
                    
                    Face3.Create(vertices.Item1.GeometryEditor, new Triangle(vertexArray[0], vertexArray[1], vertexArray[2]));
                }
            }
            else if (Selection.objects.Length == 4)
            {
                var vertices = Utilities.Get4FromSelection<Vertex>();
                if (vertices.Item1 != null && vertices.Item2 != null && vertices.Item3 != null && vertices.Item4 != null)
                {
                    var vertexArray = new[] { vertices.Item1, vertices.Item2, vertices.Item3, vertices.Item4 };
                    Utilities.ConnectUnconnectedVertices(vertexArray);
                    vertexArray = vertexArray.OrderBy(v => v.Index).ToArray();

                    
                    Face4.Create(vertices.Item1.GeometryEditor, new Triangle(vertexArray[0], vertexArray[1], vertexArray[2]), new Triangle(vertexArray[3], vertexArray[2], vertexArray[1]));
                }
            }
        }

        [MenuItem("Portal Weld/Show Hidden Objects %#&s")]
        private static void ShowHiddenObjects()
        {
            foreach (var obj in Object.FindObjectsOfType<GameObject>())
            {
                obj.hideFlags = HideFlags.None;
            }
        }

        [MenuItem("GameObject/Portal Weld/Geometry Editor", false, 11)]
        private static void CreateGeometry()
        {
            GeometryEditor.Create();
        }
    }
}
#endif
