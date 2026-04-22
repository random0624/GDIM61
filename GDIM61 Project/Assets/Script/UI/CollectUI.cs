using TMPro;
using UnityEngine;

public class CollectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectText;

    private void OnEnable()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectChanged += UpdateUI;
        }
    }

    private void Start()
    {
        if (CollectibleManager.Instance != null)
        {
            UpdateUI(
                CollectibleManager.Instance.CurrentCount,
                CollectibleManager.Instance.TotalCount
            );
        }
    }

    private void Update()
    {
        UpdateUI(CollectibleManager.Instance.CurrentCount,
                CollectibleManager.Instance.TotalCount);
    }
    private void OnDisable()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectChanged -= UpdateUI;
        }
    }

    private void UpdateUI(int current, int total)
    {
        collectText.text = current + "/" + total;
    }
}
