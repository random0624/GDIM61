using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntegrityUI : MonoBehaviour
{
    [SerializeField] private Slider integritySlider;
    [SerializeField] private float smoothDuration = 0.16f;
    [SerializeField] private float hitShakeDuration = 0.16f;
    [SerializeField] private float hitShakeStrength = 8f;
    [SerializeField] private Color hitFlashColor = new Color(1f, 0.12f, 0.08f, 1f);
    [SerializeField] private float lowIntegrityPercent = 0.3f;
    [SerializeField] private float lowPulseScale = 1.05f;
    [SerializeField] private float lowPulseSpeed = 4f;

    private Coroutine smoothRoutine;
    private Coroutine hitRoutine;
    private Image fillImage;
    private Color originalFillColor = Color.white;
    private Vector3 originalScale = Vector3.one;
    private Vector3 originalLocalPosition;
    private float previousIntegrity = -1f;
    private bool hasInitializedSlider;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        CacheFillImage();
        TrySubscribeToIntegrity();
    }

    private void Start()
    {
        TrySubscribeToIntegrity();

        if (BoatIntegrity.Instance != null)
        {
            UpdateIntegrityBar(BoatIntegrity.Instance.currentIntegrity, BoatIntegrity.Instance.maxIntegrity);
        }
    }

    private void TrySubscribeToIntegrity()
    {
        if (BoatIntegrity.Instance != null)
        {
            BoatIntegrity.Instance.OnIntegrityChanged -= UpdateIntegrityBar;
            BoatIntegrity.Instance.OnIntegrityChanged += UpdateIntegrityBar;
        }
    }

    private void OnDisable()
    {
        if (BoatIntegrity.Instance != null)
        {
            BoatIntegrity.Instance.OnIntegrityChanged -= UpdateIntegrityBar;
        }

        if (smoothRoutine != null)
        {
            StopCoroutine(smoothRoutine);
            smoothRoutine = null;
        }

        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
            hitRoutine = null;
        }

        transform.localScale = originalScale;
        transform.localPosition = originalLocalPosition;
        RestoreFillColor();
    }

    private void Update()
    {
        if (integritySlider == null || integritySlider.maxValue <= 0f)
        {
            return;
        }

        float integrityPercent = integritySlider.value / integritySlider.maxValue;
        if (integrityPercent <= lowIntegrityPercent)
        {
            float pulse = (Mathf.Sin(Time.time * lowPulseSpeed) + 1f) * 0.5f;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * lowPulseScale, pulse);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * lowPulseSpeed);
        }
    }

    private void UpdateIntegrityBar(float current, float max)
    {
        if (integritySlider == null)
        {
            return;
        }

        integritySlider.maxValue = max;

        if (!hasInitializedSlider)
        {
            integritySlider.value = current;
            previousIntegrity = current;
            hasInitializedSlider = true;
            return;
        }

        bool tookDamage = previousIntegrity >= 0f && current < previousIntegrity;
        previousIntegrity = current;

        if (smoothRoutine != null)
        {
            StopCoroutine(smoothRoutine);
        }

        smoothRoutine = StartCoroutine(SmoothSliderRoutine(current));

        if (tookDamage)
        {
            PlayHitFeedback();
        }
    }

    private void PlayHitFeedback()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
            transform.localPosition = originalLocalPosition;
            RestoreFillColor();
        }

        hitRoutine = StartCoroutine(HitFeedbackRoutine());
    }

    private IEnumerator SmoothSliderRoutine(float targetValue)
    {
        float startValue = integritySlider.value;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, smoothDuration);

        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            integritySlider.value = Mathf.Lerp(startValue, targetValue, progress);

            elapsed += Time.deltaTime;
            yield return null;
        }

        integritySlider.value = targetValue;
        smoothRoutine = null;
    }

    private IEnumerator HitFeedbackRoutine()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, hitShakeDuration);

        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);
            float intensity = 1f - progress;
            float xOffset = Random.Range(-hitShakeStrength, hitShakeStrength) * intensity;

            transform.localPosition = originalLocalPosition + new Vector3(xOffset, 0f, 0f);
            SetFillColor(Color.Lerp(originalFillColor, hitFlashColor, intensity));

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        RestoreFillColor();
        hitRoutine = null;
    }

    private void CacheFillImage()
    {
        if (integritySlider == null || integritySlider.fillRect == null)
        {
            return;
        }

        fillImage = integritySlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            originalFillColor = fillImage.color;
        }
    }

    private void SetFillColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }

    private void RestoreFillColor()
    {
        SetFillColor(originalFillColor);
    }
}
