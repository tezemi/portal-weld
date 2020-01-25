#if UNITY_EDITOR
using System.Collections.Generic;
using PortalWeld.GeometryTool;
using UnityEngine;
using UnityEditor;

namespace PortalWeld
{
    /// <summary>
    /// Contains general utility methods for portal weld.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Style for buttons that work as toggles and are currently selected.
        /// </summary>
        private static GUIStyle _selectedStyle { get; set; }
        /// <summary>
        /// Style for buttons that work as toggles and are not selected.
        /// </summary>
        private static  GUIStyle _unselectedStyle { get; set; }

        /// <summary>
        /// Works like a GUILayout method, when called in OnDrawGUI, will 
        /// draw a button that will be toggled depending on it's boolean value.
        /// </summary>
        public static bool ToggleButton(string text, bool isToggled, params GUILayoutOption[] options)
        {
            if (_unselectedStyle == null)
            {
                _unselectedStyle = new GUIStyle("Button");
            }

            if (_selectedStyle == null)
            {
                _selectedStyle = new GUIStyle(_unselectedStyle);
                _selectedStyle.normal.background = _unselectedStyle.active.background;
            }

            return GUILayout.Button(text, isToggled ? _selectedStyle : _unselectedStyle, options);
        }

        /// <summary>
        /// Returns whether or not the mono behavior's game object is selected 
        /// by the user. It does not have to be the main selection to return
        /// true.
        /// </summary>
        /// <param name="monoBehaviour">The mono behavior to check.</param>
        /// <returns>Whether or not the mono behavior is selected.</returns>
        public static bool IsSelected(MonoBehaviour monoBehaviour)
        {
            if (Selection.objects != null)
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj == monoBehaviour || obj == monoBehaviour.gameObject)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not any of the selected game objects have the 
        /// specified component type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <returns>True if any selected object has the specified component.</returns>
        public static bool IsSelected<T>()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                if (gameObject.HasComponent<T>())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Whether or not the component of the specified type is part of any 
        /// selected game object's parent
        /// </summary>
        /// <typeparam name="T">The type of component to check for.</typeparam>
        /// <returns>True if the component was found.</returns>
        public static bool IsSelectedInParent<T>()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject && gameObject.GetComponentInParent<T>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if a game object as a component. Basically an alias 
        /// for GetComponent() != null.
        /// </summary>
        /// <typeparam name="T">The type of component to check for.</typeparam>
        /// <returns>Whether or not the type of component exits on the game 
        /// object.</returns>
        public static bool HasComponent<T>(this GameObject obj)
        {
            return obj.GetComponent<T>() != null;
        }
        
        /// <summary>
        /// Converts a number within a specified range to its equivalent 
        /// position within another range.
        /// </summary>
        /// <param name="input">The number to be converted.</param>
        /// <param name="inputLow">The min of the input range.</param>
        /// <param name="inputHigh">The max of the input range.</param>
        /// <param name="outputLow">The min of the output range.</param>
        /// <param name="outputHigh">The max of the output range.</param>
        /// <returns>The number within the output range.</returns>
        public static float Range(float input, float inputLow, float inputHigh, float outputLow, float outputHigh)
        {
            return (input - inputLow) / (inputHigh - inputLow) * (outputHigh - outputLow) + outputLow;
        }

        /// <summary>
        /// Will connect any of the supplied vertices with a new edge if they 
        /// are not connected already.
        /// </summary>
        /// <param name="vertices">The vertices to connect.</param>
        public static void ConnectUnconnectedVertices(params Vertex[] vertices)
        {
            foreach (var vertex1 in vertices)
            {
                foreach (var vertex2 in vertices)
                {
                    var connectionValid = vertices.Length == 3 || vertex1.Edges.Count < 3 && vertex2.Edges.Count < 3;
                    if (vertex1 != vertex2 && !vertex1.IsConnectedTo(vertex2) && connectionValid)
                    {
                        Edge.Create(vertex1.GeometryEditor, vertex1, vertex2);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a component of the specified type from the selected game 
        /// objects.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>The component if one is selected.</returns>
        public static T GetFromSelection<T>() where T : class
        {
            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is GameObject gameObject && gameObject.HasComponent<T>())
                {
                    return gameObject.GetComponent<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets two components of the supplied type from the user's selection.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>Two components, if two are selected.</returns>
        public static (T, T) Get2FromSelection<T>() where T : class
        {
            T item1 = null;
            T item2 = null;
            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is GameObject gameObject && gameObject.HasComponent<T>())
                {
                    if (item1 == null)
                    {
                        item1 = gameObject.GetComponent<T>();
                    }
                    else
                    {
                        item2 = gameObject.GetComponent<T>();
                        break;
                    }
                }
            }

            return (item1, item2);
        }

        /// <summary>
        /// Gets three components of the supplied type from the user's selection.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>Three components, if three are selected.</returns>
        public static (T, T, T) Get3FromSelection<T>() where T : class
        {
            T item1 = null;
            T item2 = null;
            T item3 = null;
            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is GameObject gameObject && gameObject.HasComponent<T>())
                {
                    if (item1 == null)
                    {
                        item1 = gameObject.GetComponent<T>();
                    }
                    else if (item2 == null)
                    {
                        item2 = gameObject.GetComponent<T>();
                    }
                    else
                    {
                        item3 = gameObject.GetComponent<T>();
                        break;
                    }
                }
            }

            return (item1, item2, item3);
        }

        /// <summary>
        /// Gets four components of the supplied type from the user's selection.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>Four components, if four are selected.</returns>
        public static (T, T, T, T) Get4FromSelection<T>() where T : class
        {
            T item1 = null;
            T item2 = null;
            T item3 = null;
            T item4 = null;
            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is GameObject gameObject && gameObject.HasComponent<T>())
                {
                    if (item1 == null)
                    {
                        item1 = gameObject.GetComponent<T>();
                    }
                    else if (item2 == null)
                    {
                        item2 = gameObject.GetComponent<T>();
                    }
                    else if (item3 == null)
                    {
                        item3 = gameObject.GetComponent<T>();
                    }
                    else
                    {
                        item4 = gameObject.GetComponent<T>();
                        break;
                    }
                }
            }

            return (item1, item2, item3, item4);
        }

        /// <summary>
        /// Gets as many of the specified component that is currently selected.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>A list of all selected components.</returns>
        public static List<T> GetManyFromSelection<T>()
        {
            var list = new List<T>();

            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject && gameObject.HasComponent<T>())
                {
                    list.Add(gameObject.GetComponent<T>());
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a component of the specified type from the selected game 
        /// objects' parents.
        /// </summary>
        /// <typeparam name="T">The type of component tot get.</typeparam>
        /// <returns>The component if it is found.</returns>
        public static T GetFromSelectionParent<T>() where T : class
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                if (gameObject.GetComponentInParent<T>() != null)
                {
                    return gameObject.GetComponentInParent<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// Saves an asset to the editor prefs by setting its asset path as 
        /// the value. Use GetEditorPrefAsset() to get the asset.
        /// </summary>
        /// <typeparam name="T">The type of asset to save.</typeparam>
        /// <param name="key">The key to use for the asset.</param>
        /// <param name="value">The asset itself.</param>
        public static void SetEditorPrefAsset<T>(string key, T value) where T : Object
        {
            EditorPrefs.SetString(key, AssetDatabase.GetAssetPath(value));
        }

        /// <summary>
        /// Gets an asset that was saved in the editor prefs.
        /// </summary>
        /// <typeparam name="T">The type of asset to get.</typeparam>
        /// <param name="key">The key used to store the asset.</param>
        /// <param name="fallback">The path to the asset if the key isn't 
        /// found.</param>
        /// <returns>The specified asset, if it was found.</returns>
        public static T GetEditorPrefAsset<T>(string key, string fallback) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(EditorPrefs.GetString(key, fallback));
        }

        /// <summary>
        /// Gets the view side of the last scene renderer.
        /// </summary>
        /// <returns>The view side.</returns>
        public static ViewSide GetCurrentViewSide()
        {
            if (!SceneView.lastActiveSceneView.orthographic)
            {
                return ViewSide.None;
            }

            if (SceneView.lastActiveSceneView.camera.transform.rotation.eulerAngles.x == 90f)
            {
                return ViewSide.Top;
            }

            if (SceneView.lastActiveSceneView.camera.transform.rotation.eulerAngles.y == 270f)
            {
                return ViewSide.Side;
            }

            return ViewSide.Front;
        }
    }
}
#endif
