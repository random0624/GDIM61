using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private bool autoFindBoat = true;
    [SerializeField] private Vector3 offset = new Vector3(0f, 30f, 0f);
    [SerializeField] private float smoothTime = 0.35f;

    private Vector3 velocity = Vector3.zero;
    private bool followActive;

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
        followActive = true;
    }

    void LateUpdate()
    {
        if (!followActive) // Checks if the game has started
            return;

        if (target == null)
        {
            TryFindTarget();
            if (target == null)
                return;
        }

        Vector3 targetPosition = target.position + offset; // Calculates the target position

        transform.position = Vector3.SmoothDamp( // Smoothly moves the camera to the target position
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void ReturnToStart()
    {
        followActive = false;
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
}
