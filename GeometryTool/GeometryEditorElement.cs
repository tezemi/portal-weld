#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    /// <summary>
    /// Base class for elements of a geometry editor. Contains behaviors 
    /// related to drawing gizmos, movement, and grid snapping.
    /// </summary>
    [ExecuteAlways]
    public abstract class GeometryEditorElement : MonoBehaviour
    {
        /// <summary>
        /// The geometry editor this element is associated with.
        /// </summary>
        [HideInInspector]
        public GeometryEditor GeometryEditor;
        /// <summary>
        /// The position of this element during the previous frame. Used to
        /// detect movement.
        /// </summary>
        [HideInInspector]
        public Vector3 PositionLastFrame;
        /// <summary>
        /// A scale used to draw this element's gizmos relative to the 
        /// camera's distance.
        /// </summary>
        protected virtual float DynamicGizmoScale  { get; } = 0.025f;
        /// <summary>
        /// The smallest any gizmos will be drawn, even if the scene camera is 
        /// very close.
        /// </summary>
        protected virtual float MinGizmoSize { get; } = 0.15f;
        /// <summary>
        /// The largest any gizmos will be drawn, even if the scene camera is 
        /// very far away.
        /// </summary>
        protected virtual float MaxGizmoSize { get; } = 3f;
        /// <summary>
        /// The color to draw this element when it is not selected.
        /// </summary>
        protected virtual Color UnselectedGizmoColor { get; } = Color.white;
        /// <summary>
        /// The color to draw this element's gizmos when it is selected.
        /// </summary>
        protected virtual Color SelectedGizmoColor { get; } = new Color(0.25f, 0.4f, 1f);
        /// <summary>
        /// The color to draw this element's gizmos when edit mode is enabled.
        /// </summary>
        protected virtual Color ExistingGizmoColor { get; } = new Color(0.75f, 0.55f, 0.15f, 1f);
        /// <summary>
        /// The offset at which to draw text gizmos from this element.
        /// </summary>
        protected virtual Vector3 PositionTextOffset { get; } = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Gizmo size without concern for the maximum and minimum.
        /// </summary>
        protected virtual float UncappedSize => Vector3.Distance(transform.position, Camera.current.transform.position) * DynamicGizmoScale;
        
        /// <summary>
        /// The actual size to draw this element's gizmos.
        /// </summary>
        protected virtual float Size
        {
            get
            {
                var size = SceneView.lastActiveSceneView.orthographic ? 1f : Vector3.Distance(transform.position, Camera.current.transform.position) * DynamicGizmoScale;
                if (size > MaxGizmoSize)
                {
                    size = MaxGizmoSize;
                }
                else if (size < MinGizmoSize)
                {
                    size = MinGizmoSize;
                }

                return size;
            }
        } 
        
        protected virtual void Update()
        {
            if (Utilities.IsSelected(this) && transform.position != PositionLastFrame)
            {
                if (Settings.SnapToGrid && Settings.GridSize > 0f)
                {
                    SnapToGrid();                                       // If moved, snap to grid
                }

                // Do this check again after snapping to the grid, to make sure movement actually occurred
                if (transform.position != PositionLastFrame) 
                {
                    GeometryEditor.MeshPreview.UpdatePreview();         // Update the preview
                    OnMoved(transform.position - PositionLastFrame);

                    if (GeometryEditor.EditMode)
                    {
                        GeometryEditor.RebuildGeometry();               // If in edit mode, rebuild the geometry
                    }
                }
            }

            PositionLastFrame = transform.position;
        }

        protected virtual void OnDrawGizmos()
        {
            // Update the color based on mode and selection
            Gizmos.color = Utilities.IsSelected(this) ? SelectedGizmoColor : GeometryEditor.EditMode ? ExistingGizmoColor : UnselectedGizmoColor;

            // Draw position label
            if (Utilities.IsSelected(this))
            {
                Handles.Label(transform.position + PositionTextOffset, transform.position.ToString());
            }
        }

        /// <summary>
        /// Snaps this element to the grid, based on the grid's size.
        /// </summary>
        public void SnapToGrid()
        {
            var difference = transform.position - PositionLastFrame;

            transform.position = new Vector3
            (
                difference.x != 0f ? Mathf.Round(transform.position.x / Settings.GridSize) * Settings.GridSize : transform.position.x,
                difference.y != 0f ? Mathf.Round(transform.position.y / Settings.GridSize) * Settings.GridSize : transform.position.y,
                difference.z != 0f ? Mathf.Round(transform.position.z / Settings.GridSize) * Settings.GridSize : transform.position.z
            );
        }

        /// <summary>
        /// Takes the specified points and converts it to the same point but 
        /// with respect to the 2d view side.
        /// </summary>
        /// <param name="vector">The point to converte</param>
        /// <returns>The converted point.</returns>
        public Vector3 ConvertToViewPoint(Vector3 vector)
        {
            switch (Utilities.GetCurrentViewSide())
            {
                case ViewSide.None:
                    return vector;
                case ViewSide.Top:
                    return new Vector3(vector.x, SceneView.lastActiveSceneView.camera.transform.position.y - 10f, vector.z);
                case ViewSide.Front:
                    return new Vector3(vector.x, vector.y, SceneView.lastActiveSceneView.camera.transform.position.z - 10f);
                case ViewSide.Side:
                    return new Vector3(SceneView.lastActiveSceneView.camera.transform.position.x - 10f, vector.y, vector.z);
                default:
                    return vector;
            }
        }

        /// <summary>
        /// Called when this element is moved.
        /// </summary>
        /// <param name="amount">The amount the element was moved.</param>
        protected abstract void OnMoved(Vector3 amount);

        /// <summary>
        /// Called when another element part of the same editor moves.
        /// </summary>
        /// <param name="amount">The amount the other element moved.</param>
        public abstract void GeometryUpdated(Vector3 amount);

        /// <summary>
        /// Base method for creating a new geometry editor element.
        /// </summary>
        /// <typeparam name="T">The type of the element being created.</typeparam>
        /// <param name="mainGeometryEditor">The geometry editor to 
        /// assign the element to.</param>
        /// <returns>The newly created editor.</returns>
        protected static T CreateBase<T>(GeometryEditor mainGeometryEditor) where T : GeometryEditorElement
        {
            var element = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
            element.GeometryEditor = mainGeometryEditor;
            element.transform.SetParent(mainGeometryEditor.transform, true);

            return element;
        }
    }
}
#endif
