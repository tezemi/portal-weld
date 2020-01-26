#if UNITY_EDITOR
using System.Linq;
using PortalWeld.GeometryTool;
using PortalWeld.TerrainTool;
using PortalWeld.TextureTool;
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
        private const string SnapToGridPath = "Portal Weld/Geometry/Snap to Grid";
        private const string DoubleClickFacesPath = "Portal Weld/Geometry/Double Click Faces";

        static MenuCommands()
        {
            Menu.SetChecked(ShowMeshPreviewPath, Settings.ShowMeshPreview);
            Menu.SetChecked(SnapToGridPath, Settings.SnapToGrid);
            Menu.SetChecked(DoubleClickFacesPath, Settings.DoubleClickFaces);
        }
        
        [MenuItem(ShowMeshPreviewPath)]
        private static void ShowMeshPreview()
        {
            Settings.ShowMeshPreview = !Settings.ShowMeshPreview;
            Menu.SetChecked(ShowMeshPreviewPath, Settings.ShowMeshPreview);
        }

        [MenuItem(SnapToGridPath)]
        private static void SnapToGrid()
        {
            Settings.SnapToGrid = !Settings.SnapToGrid;
            Menu.SetChecked(SnapToGridPath, Settings.SnapToGrid);
        }

        [MenuItem(DoubleClickFacesPath)]
        private static void DoubleClickFaces()
        {
            Settings.DoubleClickFaces = !Settings.DoubleClickFaces;
            Menu.SetChecked(DoubleClickFacesPath, Settings.DoubleClickFaces);
        }

        [MenuItem("Portal Weld/Geometry/Bifurcate Edge")]
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

        [MenuItem("Portal Weld/Geometry/Flip Triangle(s)")]
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

        [MenuItem("Portal Weld/Geometry/Combine Faces")]
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

        [MenuItem("Portal Weld/Geometry/Create Face")]
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

        [MenuItem("Portal Weld/Geometry/Duplicate %#d")]
        private static void Duplicate()
        {
            Geometry geometry = null;
            if (Utilities.IsSelected<Geometry>())
            {
                geometry = Utilities.GetFromSelection<Geometry>();
            }
            else if (Utilities.IsSelected<EditableTexture>())
            {
                geometry = Utilities.GetFromSelectionParent<Geometry>();
            }
            else if (Utilities.IsSelected<GeometryEditor>())
            {
                var editor = Utilities.GetFromSelection<GeometryEditor>();
                if (editor.EditMode)
                {
                    geometry = editor.GeometryBeingEdited;
                }
            }
            else if (Utilities.IsSelected<GeometryEditorElement>())
            {
                var editor = Utilities.GetFromSelection<GeometryEditorElement>().GeometryEditor;
                if (editor.EditMode)
                {
                    geometry = editor.GeometryBeingEdited;
                }
            }

            if (geometry == null)
            {
                return;
            }

            var copy = Object.Instantiate(geometry.gameObject);
            var newEditor = GeometryEditor.Create(copy.GetComponent<Geometry>());
            Selection.activeGameObject = newEditor.Anchor.gameObject;
        }

        [MenuItem("Portal Weld/Geometry/Open TRS Window %#m")]
        private static void OpenTRSWindow()
        {
            TRSWindow.ShowTRSWindow();
        }

        [MenuItem("Portal Weld/Geometry/Open Terrain Window %#t")]
        private static void OpenTerrainWindow()
        {
            TerrainWindow.ShowTerrainWindow();
        }

        [MenuItem("Portal Weld/Show Hidden Objects")]
        private static void ShowHiddenObjects()
        {
            foreach (var obj in Object.FindObjectsOfType<GameObject>())
            {
                obj.hideFlags = HideFlags.None;
            }
        }

        [MenuItem("GameObject/Portal Weld/New Cube %#z", false, 11)]
        private static void CreateCube()
        {
            GeometryEditor.Create(Shape.Cube, Settings.LastBuiltGeometryPosition ?? Vector3.zero);
        }
    }
}
#endif
