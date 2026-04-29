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

            if (QuestUI.Instance != null)
            {
                QuestUI.Instance.ReportObjectiveProgress(itemID, 1);
            }

            if (CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.AddCollect();
            }

            gameObject.SetActive(false);
        }
    }
}