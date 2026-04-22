using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private bool autoFindBoat = true;
    [SerializeField] private Vector3 offset = new Vector3(0f, 30f, 0f);
    [SerializeField] private float smoothTime = 0.35f;
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

    private void Awake()
    {
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
            GameController.Instance.OnSailStarted -= HandleGameStarted;
    }

    void TrySubscribeToGameStarted()
    {
        if (GameController.Instance == null)
            return;

        GameController.Instance.OnSailStarted -= HandleGameStarted;
        GameController.Instance.OnSailStarted += HandleGameStarted;
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

        Vector3 targetPosition = target.position + offset; // Calculates the target position

        transform.position = Vector3.SmoothDamp( // Smoothly moves the camera to the target position
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        transform.rotation = Quaternion.Euler(sailViewEuler);
    }

    private void ReturnToStart()
    {
        followActive = false;
        isTransitioning = false;
        transitionTimer = 0f;
        velocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;
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

        Vector3 targetPosition = target.position + offset;
        Quaternion targetRotation = Quaternion.Euler(sailViewEuler);

        transform.position = Vector3.Lerp(transitionStartPosition, targetPosition, curvedProgress);
        transform.rotation = Quaternion.Slerp(transitionStartRotation, targetRotation, curvedProgress);

        if (progress >= 1f)
        {
            isTransitioning = false;
            followActive = true;
        }
    }
}
