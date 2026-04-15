using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float turnSpeed = 60f;
    private float currentSpeed = 0f;
    private bool controlEnabled;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    void OnEnable()
    {
        TrySubscribeToGameStarted();
    }

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        TrySubscribeToGameStarted();
    }

    public void ResetToSpawn()
    {
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        currentSpeed = 0f;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
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

        if (GameController.Instance.currentState == GameController.GameState.Sailing)
            HandleGameStarted();
    }

    void HandleGameStarted()
    {
        controlEnabled = true;
    }

    void Update()
    {
        if (!controlEnabled)
            return;

        Move();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Vertical");     // W/S
        float turnInput = Input.GetAxisRaw("Horizontal");   // A/D
        if (moveInput > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;
            BoatFuel.Instance.ConsumeFuel(acceleration * Time.deltaTime);
        }
        else if (moveInput < 0)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }

        }
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
        }
    }
}
