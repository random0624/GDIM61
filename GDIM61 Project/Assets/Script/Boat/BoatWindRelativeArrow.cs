using UnityEngine;

/// <summary>
/// Orients a child transform so its forward (+Z) matches world wind, expressed in the boat's local frame
/// (the arrow rotates with the boat in world space but always points along global wind on the horizontal plane).
/// Optional Scene view gizmo shows the same with relative bearing from bow.
/// </summary>
public class BoatWindRelativeArrow : MonoBehaviour
{
    [SerializeField] private WindZoneArea windSource;
    [Tooltip("Child of the boat; local +Z should point along the arrow mesh. Updated each frame.")]
    [SerializeField] private Transform arrowVisual;

    [Header("Gizmo")]
    [SerializeField] private float gizmoOriginYOffset = 0.75f;
    [SerializeField] private float gizmoArrowLength = 2.5f;
    [SerializeField] private bool drawGizmoWhenUnselected = true;

    private void Awake()
    {
        if (windSource == null)
            windSource = FindObjectOfType<WindZoneArea>();
    }

    private void LateUpdate()
    {
        if (arrowVisual == null || windSource == null)
            return;

        if (!TryGetHorizontalWind(out Vector3 windWorld))
            return;

        Vector3 localWind = transform.InverseTransformDirection(windWorld);
        localWind.y = 0f;
        if (localWind.sqrMagnitude < 1e-6f)
            return;

        arrowVisual.localRotation = Quaternion.LookRotation(localWind.normalized, Vector3.up);
    }

    private bool TryGetHorizontalWind(out Vector3 windWorld)
    {
        windWorld = windSource.WindDirection;
        windWorld.y = 0f;
        if (windWorld.sqrMagnitude < 1e-6f)
            return false;
        windWorld.Normalize();
        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawGizmoWhenUnselected)
            DrawWindGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmoWhenUnselected)
            DrawWindGizmo();
    }

    private void DrawWindGizmo()
    {
        if (windSource == null)
            windSource = FindObjectOfType<WindZoneArea>();
        if (windSource == null || !TryGetHorizontalWind(out Vector3 windWorld))
            return;

        Vector3 origin = transform.position + Vector3.up * gizmoOriginYOffset;
        Quaternion worldRot = Quaternion.LookRotation(windWorld, Vector3.up);

        UnityEditor.Handles.color = new Color(0.2f, 0.85f, 1f, 0.95f);
        UnityEditor.Handles.ArrowHandleCap(
            0,
            origin,
            worldRot,
            gizmoArrowLength,
            UnityEngine.EventType.Repaint);

        float signedFromBow = Vector3.SignedAngle(transform.forward, windWorld, Vector3.up);
        UnityEditor.Handles.Label(
            origin + windWorld * (gizmoArrowLength * 0.55f),
            $"Wind vs bow: {signedFromBow:F0}°");
    }
#endif
}
