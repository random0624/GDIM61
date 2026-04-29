using System.Collections;
using TMPro;
using UnityEngine;

public class CollectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectText;
    [SerializeField] private float collectPopScale = 1.55f;
    [SerializeField] private float collectPopDuration = 0.32f;
    [SerializeField] private Color collectPopColor = new Color(1f, 0.86f, 0.28f, 1f);
    [SerializeField] private float collectPopLift = 42f;
    [SerializeField] private Vector2 autoTextAnchorPosition = new Vector2(120f, -70f);

    private Coroutine popRoutine;
    private RectTransform collectTextRect;
    private Vector3 originalScale = Vector3.one;
    private Vector2 originalAnchoredPosition;
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
        EnsureCollectText();
        SetTextVisible(true);
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

        RestoreTextState();
        SetTextVisible(false);
    }

    private void UpdateUI(int current, int total)
    {
        EnsureCollectText();
        CacheOriginalState();

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
        if (collectText == null || collectTextRect == null)
        {
            return;
        }

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            RestoreTextState();
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
            collectTextRect.anchoredPosition = originalAnchoredPosition + new Vector2(0f, lift);
            collectText.color = Color.Lerp(originalColor, collectPopColor, pop);

            elapsed += Time.deltaTime;
            yield return null;
        }

        RestoreTextState();
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
            collectTextRect = collectText.GetComponent<RectTransform>();
            return;
        }

        collectText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (collectText != null)
        {
            collectTextRect = collectText.GetComponent<RectTransform>();
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        Transform textParent = canvas != null ? canvas.transform : transform;
        GameObject textObject = new GameObject("CollectText");
        textObject.transform.SetParent(textParent, false);

        collectTextRect = textObject.AddComponent<RectTransform>();
        collectTextRect.anchorMin = new Vector2(0f, 1f);
        collectTextRect.anchorMax = new Vector2(0f, 1f);
        collectTextRect.pivot = new Vector2(0f, 1f);
        collectTextRect.anchoredPosition = autoTextAnchorPosition;
        collectTextRect.sizeDelta = new Vector2(180f, 70f);

        collectText = textObject.AddComponent<TextMeshProUGUI>();
        collectText.alignment = TextAlignmentOptions.Left;
        collectText.fontSize = 56f;
        collectText.fontStyle = FontStyles.Bold;
        collectText.color = Color.white;
        collectText.raycastTarget = false;
    }

    private void SetTextVisible(bool visible)
    {
        if (collectText != null && collectText.gameObject.activeSelf != visible)
        {
            collectText.gameObject.SetActive(visible);
        }
    }

    private void CacheOriginalState()
    {
        if (collectText == null)
        {
            return;
        }

        collectTextRect = collectText.GetComponent<RectTransform>();
        originalScale = collectText.transform.localScale;
        originalColor = collectText.color;

        if (collectTextRect != null)
        {
            originalAnchoredPosition = collectTextRect.anchoredPosition;
        }
    }

    private void RestoreTextState()
    {
        if (collectText == null)
        {
            return;
        }

        collectText.transform.localScale = originalScale;
        collectText.color = originalColor;

        if (collectTextRect != null)
        {
            collectTextRect.anchoredPosition = originalAnchoredPosition;
        }
    }
}
