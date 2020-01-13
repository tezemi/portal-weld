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
        /// The actual size to draw this element's gizmos.
        /// </summary>
        protected virtual float Size => Vector3.Distance(transform.position, Camera.current.transform.position) * DynamicGizmoScale;

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
                    OnMoved(transform.position - PositionLastFrame);    // Tells all other elements that something changed

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
