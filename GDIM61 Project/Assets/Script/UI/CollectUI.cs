using System.Collections;
using TMPro;
using UnityEngine;

public class CollectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectText;
    [SerializeField] private float collectPopScale = 1.55f;
    [SerializeField] private float collectPopDuration = 0.32f;
    [SerializeField] private Color collectPopColor = new Color(1f, 0.86f, 0.28f, 1f);
    [SerializeField] private float collectPopLift = 18f;

    private Coroutine popRoutine;
    private Vector3 originalScale = Vector3.one;
    private Vector3 originalLocalPosition;
    private Color originalColor = Color.white;
    private int lastCurrent = -1;
    private bool isSubscribed;

    private void Awake()
    {
        EnsureCollectText();
        CacheOriginalState();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        EnsureCollectText();
        CacheOriginalState();
        TrySubscribe();

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
        if (CollectibleManager.Instance != null && isSubscribed)
        {
            CollectibleManager.Instance.OnCollectChanged -= UpdateUI;
        }
        isSubscribed = false;

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            popRoutine = null;
        }

        if (collectText != null)
        {
            collectText.transform.localScale = originalScale;
            collectText.transform.localPosition = originalLocalPosition;
            collectText.color = originalColor;
        }
    }

    private void UpdateUI(int current, int total)
    {
        EnsureCollectText();

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
            float lift = Mathf.Sin(progress * Mathf.PI) * collectPopLift;
            collectText.transform.localScale = Vector3.Lerp(originalScale, targetScale, pop);
            collectText.transform.localPosition = originalLocalPosition + new Vector3(0f, lift, 0f);
            collectText.color = Color.Lerp(originalColor, collectPopColor, pop);

            elapsed += Time.deltaTime;
            yield return null;
        }

        collectText.transform.localScale = originalScale;
        collectText.transform.localPosition = originalLocalPosition;
        collectText.color = originalColor;
        popRoutine = null;
    }

    private void TrySubscribe()
    {
        if (CollectibleManager.Instance == null || isSubscribed)
        {
            return;
        }

        CollectibleManager.Instance.OnCollectChanged -= UpdateUI;
        CollectibleManager.Instance.OnCollectChanged += UpdateUI;
        isSubscribed = true;
    }

    private void EnsureCollectText()
    {
        if (collectText != null)
        {
            return;
        }

        collectText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (collectText != null)
        {
            return;
        }

        GameObject textObject = new GameObject("CollectText");
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        collectText = textObject.AddComponent<TextMeshProUGUI>();
        collectText.alignment = TextAlignmentOptions.Center;
        collectText.fontSize = 42f;
        collectText.fontStyle = FontStyles.Bold;
        collectText.color = Color.white;
        collectText.raycastTarget = false;
    }

    private void CacheOriginalState()
    {
        if (collectText == null)
        {
            return;
        }

        originalScale = collectText.transform.localScale;
        originalLocalPosition = collectText.transform.localPosition;
        originalColor = collectText.color;
    }
}
