using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] private Transform target; 
    [SerializeField] private bool autoFindBoat = true;
    [SerializeField] private Vector3 offset = new Vector3(0f, 30f, 0f);
    [SerializeField] private float smoothTime = 0.35f;

    [Header("Hit Shake")]
    [SerializeField] private float hitShakeDuration = 0.16f;
    [SerializeField] private float hitShakeStrength = 0.25f;

    [Header("Sail Transition")]
    [SerializeField] private float sailTransitionDuration = 1.5f;
    [SerializeField] private Vector3 sailViewEuler = new Vector3(90f, 0f, 0f);
    [SerializeField] private AnimationCurve sailTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 velocity = Vector3.zero;
    private bool followActive;
    private bool isTransitioning;
    private float transitionTimer;
    private Vector3 transitionStartPosition;
    private Quaternion transitionStartRotation;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 currentHitShakeOffset;
    private Vector3 appliedHitShakeOffset;
    private Coroutine hitShakeRoutine;

    private void Awake()
    {
        Instance = this;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable()
    {
        TrySubscribeToGameStarted();
    }

    void Start()
    {
        TryFindTarget();
        TrySubscribeToGameStarted();
    }

    void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted -= HandleGameStarted;
            GameController.Instance.OnMainMenuStarted -= ReturnToStart;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void TrySubscribeToGameStarted()
    {
        if (GameController.Instance == null)
            return;

        GameController.Instance.OnSailStarted -= HandleGameStarted;
        GameController.Instance.OnSailStarted += HandleGameStarted;
        GameController.Instance.OnMainMenuStarted -= ReturnToStart;
        GameController.Instance.OnMainMenuStarted += ReturnToStart;

        if (GameController.Instance.currentState == GameController.GameState.Sailing)
            HandleGameStarted();
    }

    void HandleGameStarted()
    {
        TryFindTarget();
        if (target == null)
            return;

        transitionStartPosition = transform.position;
        transitionStartRotation = transform.rotation;
        transitionTimer = 0f;
        isTransitioning = true;
        followActive = false;
        velocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            TryFindTarget();
            if (target == null && (followActive || isTransitioning))
                return;
        }

        if (isTransitioning)
        {
            UpdateSailTransition();
            return;
        }

        if (!followActive) // Checks if the game has started
            return;

        ClearAppliedHitShakeOffset();
        Vector3 targetPosition = target.position + offset; // Calculates the target position

        transform.position = Vector3.SmoothDamp( // Smoothly moves the camera to the target position
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
        ApplyHitShakeOffset();

        transform.rotation = Quaternion.Euler(sailViewEuler);
    }

    private void ReturnToStart()
    {
        followActive = false;
        isTransitioning = false;
        transitionTimer = 0f;
        velocity = Vector3.zero;
        currentHitShakeOffset = Vector3.zero;
        appliedHitShakeOffset = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    public void PlayHitShake()
    {
        if (hitShakeRoutine != null)
        {
            StopCoroutine(hitShakeRoutine);
        }

        hitShakeRoutine = StartCoroutine(PlayHitShakeRoutine());
    }

    public static void PlayHitShakeOnCamera()
    {
        if (Instance != null)
        {
            Instance.PlayHitShake();
        }
    }

    private IEnumerator PlayHitShakeRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, hitShakeDuration);

        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float intensity = 1f - progress;
            Vector2 randomCircle = Random.insideUnitCircle * hitShakeStrength * intensity;
            currentHitShakeOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentHitShakeOffset = Vector3.zero;
        hitShakeRoutine = null;
    }

    private void ClearAppliedHitShakeOffset()
    {
        if (appliedHitShakeOffset == Vector3.zero)
        {
            return;
        }

        transform.position -= appliedHitShakeOffset;
        appliedHitShakeOffset = Vector3.zero;
    }

    private void ApplyHitShakeOffset()
    {
        transform.position += currentHitShakeOffset;
        appliedHitShakeOffset = currentHitShakeOffset;
    }

    private void TryFindTarget()
    {
        if (!autoFindBoat || target != null)
            return;

        BoatController boatController = FindObjectOfType<BoatController>();
        if (boatController != null)
        {
            target = boatController.transform;
        }
    }

    private void UpdateSailTransition()
    {
        if (target == null)
        {
            isTransitioning = false;
            return;
        }

        transitionTimer += Time.deltaTime;
        float duration = Mathf.Max(0.01f, sailTransitionDuration);
        float progress = Mathf.Clamp01(transitionTimer / duration);
        float curvedProgress = sailTransitionCurve.Evaluate(progress);

        ClearAppliedHitShakeOffset();
        Vector3 targetPosition = target.position + offset;
        Quaternion targetRotation = Quaternion.Euler(sailViewEuler);

        transform.position = Vector3.Lerp(transitionStartPosition, targetPosition, curvedProgress);
        ApplyHitShakeOffset();
        transform.rotation = Quaternion.Slerp(transitionStartRotation, targetRotation, curvedProgress);

        if (progress >= 1f)
        {
            isTransitioning = false;
            followActive = true;
        }
    }
}
