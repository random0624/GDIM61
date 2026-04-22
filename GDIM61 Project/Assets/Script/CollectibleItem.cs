using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.AddCollect();
            }

            gameObject.SetActive(false);
        }
    }
}