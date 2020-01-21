#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    public class TRSWindow : EditorWindow
    {
        private Vector3 _translation;
        private Vector3 _rotation;
        private Vector3 _scale;

        public static void ShowTRSWindow()
        {
            GetWindow(typeof(TRSWindow), true, "Translate, Rotate, Scale", true);
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            _translation = EditorGUILayout.Vector3Field("Translation:", _translation);
            _rotation = EditorGUILayout.Vector3Field("Rotation:", _rotation);
            _scale = EditorGUILayout.Vector3Field("Scale:", _scale);

            if (GUILayout.Button("Okay"))
            {
                if (Utilities.IsSelected<GeometryEditorElement>())
                {
                    var editor = Utilities.GetFromSelection<GeometryEditorElement>().GeometryEditor;

                    editor.Anchor.transform.position += _translation;

                    foreach (var vertex in editor.Vertices)
                    {
                        vertex.transform.RotateAround(editor.Anchor.transform.position, Vector3.forward, _rotation.x);
                        vertex.transform.RotateAround(editor.Anchor.transform.position, Vector3.up, _rotation.y);
                        vertex.transform.RotateAround(editor.Anchor.transform.position, Vector3.right, _rotation.z);

                        var difference = (editor.Anchor.transform.position - vertex.transform.position);
                        vertex.transform.position += new Vector3(difference.x * _scale.x, difference.y * _scale.y, difference.z * _scale.z);
                    }

                    editor.MeshPreview.UpdatePreview();
                    if (editor.EditMode)
                    {
                        editor.RebuildGeometry();
                    }
                }

                Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
