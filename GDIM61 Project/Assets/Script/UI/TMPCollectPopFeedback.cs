using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TMPCollectPopFeedback : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private float popScale = 1.55f;
    [SerializeField] private float popDuration = 0.32f;
    [SerializeField] private Color popColor = new Color(1f, 0.86f, 0.28f, 1f);
    [SerializeField] private float popLift = 42f;

    private Coroutine popRoutine;
    private RectTransform rectTransform;
    private Vector3 originalScale = Vector3.one;
    private Vector2 originalAnchoredPosition;
    private Color originalColor = Color.white;

    private void Awake()
    {
        EnsureTarget();
        CacheOriginalState();
    }

    private void OnDisable()
    {
        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            popRoutine = null;
        }

        RestoreTextState();
    }

    public void CacheBaseline()
    {
        EnsureTarget();
        CacheOriginalState();
    }

    public void PlayPop()
    {
        EnsureTarget();
        if (targetText == null || rectTransform == null)
        {
            return;
        }

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            RestoreTextState();
        }

        popRoutine = StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, popDuration);
        Vector3 targetScale = originalScale * popScale;

        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float envelope = Mathf.Sin(progress * Mathf.PI);
            float lift = envelope * popLift;

            targetText.transform.localScale = Vector3.Lerp(originalScale, targetScale, envelope);
            rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(0f, lift);
            targetText.color = Color.Lerp(originalColor, popColor, envelope);

            elapsed += Time.deltaTime;
            yield return null;
        }

        RestoreTextState();
        popRoutine = null;
    }

    private void EnsureTarget()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
        }

        rectTransform = targetText != null ? targetText.rectTransform : GetComponent<RectTransform>();
    }

    private void CacheOriginalState()
    {
        if (targetText == null || rectTransform == null)
        {
            return;
        }

        originalScale = targetText.transform.localScale;
        originalColor = targetText.color;
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    private void RestoreTextState()
    {
        if (targetText == null)
        {
            return;
        }

        targetText.transform.localScale = originalScale;
        targetText.color = originalColor;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }
}
