using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float turnSpeed = 60f;

    public float currentSpeed = 0f;

    private bool controlEnabled;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    [Header("UI")]
    [SerializeField] private GameObject successPanel;

    [Header("Sail")]
    [SerializeField] private Transform sailTransform;
    [SerializeField] private float sailRotateSpeed = 60f;
    [SerializeField] private float maxSailAngle = 80f;
    [SerializeField] private float maxSailBoost = 2f;

    private float currentSailAngle = 0f;
    private Vector3 currentWindDirection = Vector3.zero;
    private float currentWindStrength = 0f;

    [Header("Wind")]
    [SerializeField] private WindZoneArea globalWind;

    [Header("Island Damage")]
    [SerializeField] private float damageMultiplier = 3f;
    [SerializeField] private float minIslandCollisionDamage = 5f;
    [SerializeField] private float islandDamageCooldown = 0.45f;
    [SerializeField] private Vector3 islandHitCheckHalfExtents = new Vector3(0.75f, 4f, 0.95f);

    private float lastIslandDamageTime = -999f;
    private bool wasTouchingIsland;
    private readonly Collider[] islandHitResults = new Collider[12];

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        if (GetComponent<BoatHitFeedback>() == null)
        {
            gameObject.AddComponent<BoatHitFeedback>();
        }

        if (globalWind == null)
        {
            globalWind = FindObjectOfType<WindZoneArea>();
        }
    }

    private void OnEnable()
    {
        TrySubscribeToGameController();
    }

    private void Start()
    {
        TrySubscribeToGameController();
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnSailStarted -= HandleSailStarted;
            GameController.Instance.OnMainMenuStarted -= ResetToSpawn;
        }
    }

    private void TrySubscribeToGameController()
    {
        if (GameController.Instance == null)
            return;

        GameController.Instance.OnSailStarted -= HandleSailStarted;
        GameController.Instance.OnSailStarted += HandleSailStarted;

        GameController.Instance.OnMainMenuStarted -= ResetToSpawn;
        GameController.Instance.OnMainMenuStarted += ResetToSpawn;

        if (GameController.Instance.CurrentState == GameController.GameState.Sailing)
        {
            HandleSailStarted();
        }
    }

    private void HandleSailStarted()
    {
        controlEnabled = true;
    }

    private void ResetToSpawn()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        currentSpeed = 0f;
        currentSailAngle = 0f;
        currentWindDirection = Vector3.zero;
        currentWindStrength = 0f;
        wasTouchingIsland = false;

        if (sailTransform != null)
        {
            sailTransform.localRotation = Quaternion.identity;
        }

        controlEnabled = false;
    }

    private void Update()
    {
        if (!controlEnabled)
            return;

        if (GameController.Instance == null ||
            GameController.Instance.CurrentState != GameController.GameState.Sailing)
            return;

        SailRotation();
        Move();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Vertical");
        float turnInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {
            currentSpeed += acceleration * Time.deltaTime;

            if (BoatFuel.Instance != null)
            {
                BoatFuel.Instance.ConsumeFuel(acceleration * Time.deltaTime);
            }
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
                currentSpeed = Mathf.Max(currentSpeed, 0f);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0f);
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        SyncGlobalWind();

        Vector3 playerMove = transform.forward * currentSpeed;
        Vector3 sailForce = CalculateSailForce();
        Vector3 finalMove = playerMove + sailForce;

        transform.position += finalMove * Time.deltaTime;

        if (finalMove.magnitude > 0.1f)
        {
            transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
        }

        CheckIslandHit();
    }

    private void SailRotation()
    {
        float sailInput = 0f;

        if (Input.GetKey(KeyCode.Q))
            sailInput = -1f;
        else if (Input.GetKey(KeyCode.E))
            sailInput = 1f;

        currentSailAngle += sailInput * sailRotateSpeed * Time.deltaTime;
        currentSailAngle = Mathf.Clamp(currentSailAngle, -maxSailAngle, maxSailAngle);

        if (sailTransform != null)
        {
            sailTransform.localRotation = Quaternion.Euler(0f, currentSailAngle, 0f);
        }
    }

    private void SyncGlobalWind()
    {
        if (globalWind != null)
        {
            currentWindDirection = globalWind.WindDirection;
            currentWindStrength = globalWind.WindStrength;
        }
        else
        {
            currentWindDirection = Vector3.zero;
            currentWindStrength = 0f;
        }
    }

    private Vector3 CalculateSailForce()
    {
        if (sailTransform == null)
            return Vector3.zero;

        if (currentWindStrength <= 0f || currentWindDirection == Vector3.zero)
            return Vector3.zero;

        Vector3 windDir = currentWindDirection.normalized;
        Vector3 sailRight = sailTransform.right.normalized;
        Vector3 boatForward = transform.forward.normalized;

        float angle = Vector3.Angle(sailRight, windDir);
        float efficiency = Mathf.Sin(angle * Mathf.Deg2Rad);
        efficiency = Mathf.Clamp01(efficiency);

        float forceAmount = efficiency * currentWindStrength * maxSailBoost;

        Vector3 rawForce = windDir * forceAmount;

        Vector3 sideForce = rawForce * 0.3f;

        float forwardAmount = Mathf.Max(0f, Vector3.Dot(rawForce.normalized, boatForward));
        Vector3 forwardForce = boatForward * forwardAmount * rawForce.magnitude;

        return forwardForce + sideForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsIslandObject(collision.transform))
        {
            DamageBoatFromIsland();
        }
    }

    private void CheckIslandHit()
    {
        int hitCount = Physics.OverlapBoxNonAlloc(
            transform.position,
            islandHitCheckHalfExtents,
            islandHitResults,
            transform.rotation,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        bool isTouchingIsland = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = islandHitResults[i];

            if (hit == null)
                continue;

            if (hit.GetComponentInParent<BoatController>() == this)
                continue;

            if (IsIslandObject(hit.transform))
            {
                isTouchingIsland = true;
                break;
            }
        }

        if (isTouchingIsland && !wasTouchingIsland)
        {
            DamageBoatFromIsland();
        }

        wasTouchingIsland = isTouchingIsland;
    }

    private void DamageBoatFromIsland()
    {
        if (Time.time - lastIslandDamageTime < islandDamageCooldown)
            return;

        float impactForce = Mathf.Abs(currentSpeed);
        float damage = Mathf.Max(impactForce * damageMultiplier, minIslandCollisionDamage);

        if (BoatIntegrity.Instance != null && damage > 0f)
        {
            lastIslandDamageTime = Time.time;
            BoatIntegrity.Instance.ConsumeIntegrity(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HomePoint"))
        {
            if (GameController.Instance != null)
            {
                GameController.Instance.ChangeState(GameController.GameState.MainMenu);
            }

            if (BoatFuel.Instance != null)
            {
                BoatFuel.Instance.Refill();
            }

            if (BoatIntegrity.Instance != null)
            {
                BoatIntegrity.Instance.HealIntegrity();
            }

            if (successPanel != null)
            {
                successPanel.SetActive(true);
            }
        }
    }

    private bool IsIslandObject(Transform hitTransform)
    {
        while (hitTransform != null)
        {
            if (hitTransform.CompareTag("Island"))
            {
                return true;
            }

            hitTransform = hitTransform.parent;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, islandHitCheckHalfExtents * 2f);
    }
}
