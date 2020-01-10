#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PortalWeld.GeometryTool
{
    [ExecuteAlways]
    public abstract class GeometryObject : MonoBehaviour
    {
        [HideInInspector]
        public GeometryEditor GeometryEditor;
        [HideInInspector]
        public Vector3 PositionLastFrame;
        protected virtual float MaximumGizoSize { get; } = 0.5f;
        protected virtual float DynamicGizmoScale  { get; } = 0.025f;
        protected virtual float MinimumGizmoSize => MaximumGizoSize / 3f;
        protected virtual Color UnselectedGizmoColor { get; } = Color.white;
        protected virtual Color SelectedGizmoColor { get; } = new Color(0.25f, 0.4f, 1f);
        protected virtual Color ExistingGizmoColor { get; } = new Color(0.75f, 0.55f, 0.15f, 1f);
        protected virtual Vector3 PositionTextOffset { get; } = new Vector3(1f, 1f, 1f);
        protected virtual float Size => Settings.ShowDynamicGizmos ? Vector3.Distance(transform.position, Camera.current.transform.position) * DynamicGizmoScale : MaximumGizoSize;

        protected virtual void Update()
        {
            if (Utilities.IsSelected(this) && transform.position != PositionLastFrame)
            {
                if (Settings.SnapToGrid && Settings.GridSize > 0f)
                {
                    SnapToGrid();
                }

                GeometryEditor.MeshPreview.UpdatePreview();
                OnMoved(transform.position - PositionLastFrame);
            }

            PositionLastFrame = transform.position;
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Utilities.IsSelected(this) ? SelectedGizmoColor : GeometryEditor.EditType == GeometryEditType.Existing ? ExistingGizmoColor : UnselectedGizmoColor;
            if (Utilities.IsSelected(this))
            {
                Handles.Label(transform.position + PositionTextOffset, transform.position.ToString());
            }
        }

        public void SnapToGrid()
        {
            transform.position = new Vector3
            (
                Mathf.Round(transform.position.x / Settings.GridSize) * Settings.GridSize,
                Mathf.Round(transform.position.y / Settings.GridSize) * Settings.GridSize,
                Mathf.Round(transform.position.z / Settings.GridSize) * Settings.GridSize
            );
        }

        protected abstract void OnMoved(Vector3 amount);

        public abstract void GeometryUpdated(Vector3 amount);

        protected static T CreateBase<T>(GeometryEditor mainGeometryEditor) where T : GeometryObject
        {
            var element = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
            element.GeometryEditor = mainGeometryEditor;
            element.transform.SetParent(mainGeometryEditor.transform, true);

            return element;
        }
    }
}
#endif
