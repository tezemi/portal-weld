#if UNITY_EDITOR
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
    }
}
#endif
