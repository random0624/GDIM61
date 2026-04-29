using System.Collections;
using TMPro;
using UnityEngine;

public class CollectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectText;
    [SerializeField] private float collectPopScale = 1.22f;
    [SerializeField] private float collectPopDuration = 0.18f;

    private Coroutine popRoutine;
    private Vector3 originalScale = Vector3.one;
    private int lastCurrent = -1;

    private void Awake()
    {
        if (collectText != null)
        {
            originalScale = collectText.transform.localScale;
        }
    }

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

    private void OnDisable()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.OnCollectChanged -= UpdateUI;
        }

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            popRoutine = null;
        }

        if (collectText != null)
        {
            collectText.transform.localScale = originalScale;
        }
    }

    private void UpdateUI(int current, int total)
    {
        if (collectText == null)
        {
            return;
        }

        collectText.text = current + "/" + total;

        if (lastCurrent >= 0 && current > lastCurrent)
        {
            PlayCollectPop();
        }

        lastCurrent = current;
    }

    private void PlayCollectPop()
    {
        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
        }

        popRoutine = StartCoroutine(CollectPopRoutine());
    }

    private IEnumerator CollectPopRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, collectPopDuration);
        Vector3 targetScale = originalScale * collectPopScale;

        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float pop = Mathf.Sin(progress * Mathf.PI);
            collectText.transform.localScale = Vector3.Lerp(originalScale, targetScale, pop);

            elapsed += Time.deltaTime;
            yield return null;
        }

        collectText.transform.localScale = originalScale;
        popRoutine = null;
    }
}
