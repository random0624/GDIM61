using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float smoothTime = 0.35f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 相机不跟着船转，固定朝下
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}