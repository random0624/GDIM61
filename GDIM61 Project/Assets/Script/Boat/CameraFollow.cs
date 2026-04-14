using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float smoothTime = 0.35f;

    private Vector3 velocity = Vector3.zero;
    private bool followActive;

    void OnEnable()
    {
        TrySubscribeToGameStarted();
    }

    void Start()
    {
        TrySubscribeToGameStarted();
    }

    void OnDisable()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnGameStarted -= HandleGameStarted;
    }

    void TrySubscribeToGameStarted()
    {
        if (GameController.Instance == null)
            return;

        GameController.Instance.OnGameStarted -= HandleGameStarted;
        GameController.Instance.OnGameStarted += HandleGameStarted;

        if (GameController.Instance.IsGameStarted)
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

        Vector3 targetPosition = target.position + offset; // Calculates the target position

        transform.position = Vector3.SmoothDamp( // Smoothly moves the camera to the target position
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // ��������Ŵ�ת���̶�����
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}