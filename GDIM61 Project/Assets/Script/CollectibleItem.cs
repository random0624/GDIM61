using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemID; // Unique identifier for the collectible item
    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.AddChest();
            }
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(Random.Range(1,20));
            }

            gameObject.SetActive(false);
        }
    }
}