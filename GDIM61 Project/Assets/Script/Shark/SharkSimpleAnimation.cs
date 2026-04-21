using UnityEngine;

public class SharkSimpleAnimation : MonoBehaviour
{
    [Header("Movement Source")]
    [SerializeField] private Transform movementSource;

    [Header("Idle Motion")]
    [SerializeField] private float idleBobHeight = 0.08f;
    [SerializeField] private float idleBobSpeed = 1.5f;
    [SerializeField] private float idleSwayAngle = 6f;
    [SerializeField] private float idleSwaySpeed = 2f;

    [Header("Swim Motion")]
    [SerializeField] private float swimBobHeight = 0.14f;
    [SerializeField] private float swimBobSpeed = 3f;
    [SerializeField] private float swimSwayAngle = 14f;
    [SerializeField] private float swimSwaySpeed = 5f;
    [SerializeField] private float speedForFullSwim = 8f;
    [SerializeField] private float animationBlendSpeed = 4f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 previousSourcePosition;
    private float swimBlend;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        if (movementSource == null)
        {
            movementSource = transform.parent != null ? transform.parent : transform;
        }

        previousSourcePosition = movementSource.position;
    }

    private void Update()
    {
        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
        float currentSpeed = Vector3.Distance(movementSource.position, previousSourcePosition) / deltaTime;
        previousSourcePosition = movementSource.position;

        float targetBlend = Mathf.Clamp01(currentSpeed / Mathf.Max(speedForFullSwim, 0.01f));
        swimBlend = Mathf.MoveTowards(swimBlend, targetBlend, animationBlendSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        float bobHeight = Mathf.Lerp(idleBobHeight, swimBobHeight, swimBlend);
        float bobSpeed = Mathf.Lerp(idleBobSpeed, swimBobSpeed, swimBlend);
        float swayAngle = Mathf.Lerp(idleSwayAngle, swimSwayAngle, swimBlend);
        float swaySpeed = Mathf.Lerp(idleSwaySpeed, swimSwaySpeed, swimBlend);

        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        float swayOffset = Mathf.Sin(Time.time * swaySpeed) * swayAngle;

        transform.localPosition = initialLocalPosition + new Vector3(0f, bobOffset, 0f);
        transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, swayOffset, 0f);
    }
}
