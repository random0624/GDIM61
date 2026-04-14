using UnityEngine;

public class BoatCollision : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Island"))
        {

            float impactForce = collision.relativeVelocity.magnitude;
            float damage = impactForce * damageMultiplier;

            Debug.Log($"撞击岛屿！力度：{impactForce}，造成伤害：{damage}");

            if (BoatIntegrity.Instance != null)
            {
                BoatIntegrity.Instance.ConsumeIntegrity(damage);
            }
        }
    }
}