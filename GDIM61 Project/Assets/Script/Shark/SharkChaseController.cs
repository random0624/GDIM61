using UnityEngine;

public class SharkChaseController : MonoBehaviour
{
    private enum SharkState
    {
        Idle,
        Alert,
        Chasing,
        Returning
    }

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindBoat = true;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private float alertDuration = 2f;
    [SerializeField] private GameObject alertIcon;

    [Header("Chase")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float chaseDuration = 8f;
    [SerializeField] private float catchDistance = 2.5f;
    [SerializeField] private float catchDamage = 20f;

    [Header("Return")]
    [SerializeField] private float returnStopDistance = 0.5f;

    private SharkState currentState = SharkState.Idle;
    private float alertTimer;
    private float chaseTimer;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Quaternion modelRotationOffset;

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        modelRotationOffset = Quaternion.Inverse(Quaternion.Euler(0f, spawnRotation.eulerAngles.y, 0f)) * spawnRotation;
        SetAlertIcon(false);
    }

    private void Start()
    {
        TryFindBoat();
    }

    private void Update()
    {
        if (GameController.Instance != null && GameController.Instance.currentState != GameController.GameState.Sailing)
        {
            ResetShark();
            return;
        }

        if (target == null)
        {
            TryFindBoat();
            return;
        }

        switch (currentState)
        {
            case SharkState.Idle:
                UpdateIdle();
                break;
            case SharkState.Alert:
                UpdateAlert();
                break;
            case SharkState.Chasing:
                UpdateChasing();
                break;
            case SharkState.Returning:
                UpdateReturning();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (IsTargetInDetectionRange())
        {
            currentState = SharkState.Alert;
            alertTimer = 0f;
            SetAlertIcon(true);
        }
    }

    private void UpdateAlert()
    {
        if (!IsTargetInDetectionRange())
        {
            currentState = SharkState.Returning;
            SetAlertIcon(false);
            return;
        }

        alertTimer += Time.deltaTime;
        LookAtTarget(target.position);

        if (alertTimer >= alertDuration)
        {
            currentState = SharkState.Chasing;
            chaseTimer = 0f;
            SetAlertIcon(false);
        }
    }

    private void UpdateChasing()
    {
        chaseTimer += Time.deltaTime;
        MoveTowards(target.position);

        if (Vector3.Distance(transform.position, target.position) <= catchDistance)
        {
            if (BoatIntegrity.Instance != null && catchDamage > 0f)
            {
                BoatIntegrity.Instance.ConsumeIntegrity(catchDamage);
            }

            currentState = SharkState.Returning;
            return;
        }

        if (chaseTimer >= chaseDuration)
        {
            currentState = SharkState.Returning;
        }
    }

    private void UpdateReturning()
    {
        MoveTowards(spawnPosition);

        if (Vector3.Distance(transform.position, spawnPosition) <= returnStopDistance)
        {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            currentState = SharkState.Idle;
        }
    }

    private void MoveTowards(Vector3 destination)
    {
        LookAtTarget(destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private void LookAtTarget(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up) * modelRotationOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsTargetInDetectionRange()
    {
        return Vector3.Distance(transform.position, target.position) <= detectionRadius;
    }

    private void TryFindBoat()
    {
        if (!autoFindBoat || target != null)
        {
            return;
        }

        BoatController boatController = FindObjectOfType<BoatController>();
        if (boatController != null)
        {
            target = boatController.transform;
        }
    }

    private void ResetShark()
    {
        currentState = SharkState.Idle;
        alertTimer = 0f;
        chaseTimer = 0f;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        SetAlertIcon(false);
    }

    private void SetAlertIcon(bool shouldShow)
    {
        if (alertIcon != null && alertIcon.activeSelf != shouldShow)
        {
            alertIcon.SetActive(shouldShow);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}
