using UnityEngine;

/// <summary>
/// Global wind in <b>world space</b>: direction does not depend on this GameObject's position or rotation.
/// Strength is briefly zero right after sailing begins so the boat does not lurch before the first frame settles.
/// </summary>
public class WindZoneArea : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("World Y rotation of the blow direction. 0° = toward world +Z, 90° = toward +X. Independent of this object's transform.")]
    [SerializeField] private float worldWindYawDegrees = 0f;
    [SerializeField] private float windStrength = 5f;

    [Header("Startup")]
    [Tooltip("Seconds after entering Sailing before wind strength applies (avoids initial forward shove).")]
    [SerializeField] private float windStrengthDelayAfterSailStart = 0.2f;

    private float windStrengthSuppressedUntil;

    /// <summary>Horizontal unit vector in world space; never rotates with the boat.</summary>
    public Vector3 WindDirection => Quaternion.Euler(0f, worldWindYawDegrees, 0f) * Vector3.forward;

    public float WindStrength => GetEffectiveWindStrength();

    private void Awake()
    {
        SubscribeGameController();
    }

    private void Start()
    {
        SubscribeGameController();
        // Handles play mode starting already in Sailing, or GameController firing before this Awake.
        if (GameController.Instance != null &&
            GameController.Instance.CurrentState == GameController.GameState.Sailing)
        {
            ArmWindDelay();
        }
    }

    private void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted -= HandleSailStarted;
            GameController.Instance.OnMainMenuStarted -= HandleMainMenuStarted;
        }
    }

    private void SubscribeGameController()
    {
        if (GameController.Instance == null)
            return;

        GameController.Instance.OnSailStarted -= HandleSailStarted;
        GameController.Instance.OnSailStarted += HandleSailStarted;
        GameController.Instance.OnMainMenuStarted -= HandleMainMenuStarted;
        GameController.Instance.OnMainMenuStarted += HandleMainMenuStarted;
    }

    private void HandleSailStarted()
    {
        ArmWindDelay();
    }

    private void HandleMainMenuStarted()
    {
        windStrengthSuppressedUntil = 0f;
    }

    private void ArmWindDelay()
    {
        windStrengthSuppressedUntil = Time.time + Mathf.Max(0f, windStrengthDelayAfterSailStart);
    }

    private float GetEffectiveWindStrength()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.CurrentState != GameController.GameState.Sailing)
                return 0f;

            if (Time.time < windStrengthSuppressedUntil)
                return 0f;
        }

        return windStrength;
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = WindDirection;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, dir * 3f);
    }
}
