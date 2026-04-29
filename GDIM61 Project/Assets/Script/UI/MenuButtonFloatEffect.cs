using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonFloatEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Float")]
    [SerializeField] private float horizontalFloatAmplitude = 8f;
    [SerializeField] private float verticalFloatAmplitude = 10f;
    [SerializeField] private float horizontalFloatSpeed = 1.1f;
    [SerializeField] private float verticalFloatSpeed = 1.6f;

    [Header("Hover")]
    [SerializeField] private float hoverScale = 1.16f;
    [SerializeField] private float hoverLift = 14f;
    [SerializeField] private float hoverLerpSpeed = 12f;
    [SerializeField] private Color hoverTint = new Color(1f, 0.93f, 0.62f, 1f);
    [SerializeField] private float hoverBrightness = 1.35f;
    [SerializeField] private float hoverWobbleAngle = 3.5f;
    [SerializeField] private float hoverWobbleSpeed = 9f;

    [Header("Glow")]
    [SerializeField] private Color glowColor = new Color(1f, 0.84f, 0.35f, 0.85f);
    [SerializeField] private Vector2 glowDistance = new Vector2(3f, -3f);
    [SerializeField] private Color shadowColor = new Color(0f, 0.25f, 0.38f, 0.45f);
    [SerializeField] private Vector2 shadowDistance = new Vector2(8f, -8f);

    private RectTransform rectTransform;
    private Graphic targetGraphic;
    private Outline hoverOutline;
    private Shadow hoverShadow;
    private Vector2 startAnchoredPosition;
    private Vector3 startScale;
    private Quaternion startRotation;
    private Color startColor = Color.white;
    private bool isHovering;
    private float horizontalOffset;
    private float verticalOffset;
    private float hoverAmount;
    private float hoverPunch;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetGraphic = GetComponent<Graphic>();

        if (targetGraphic == null)
        {
            targetGraphic = GetComponentInChildren<Graphic>();
        }

        hoverOutline = GetComponent<Outline>();
        if (hoverOutline == null)
        {
            hoverOutline = gameObject.AddComponent<Outline>();
        }

        hoverShadow = GetShadowComponent();
        if (hoverShadow == null)
        {
            hoverShadow = gameObject.AddComponent<Shadow>();
        }
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        startAnchoredPosition = rectTransform.anchoredPosition;
        startScale = rectTransform.localScale;
        startRotation = rectTransform.localRotation;
        horizontalOffset = Random.Range(0f, Mathf.PI * 2f);
        verticalOffset = Random.Range(0f, Mathf.PI * 2f);
        isHovering = false;
        hoverAmount = 0f;
        hoverPunch = 0f;

        if (targetGraphic != null)
        {
            startColor = targetGraphic.color;
        }

        SetEffectAlpha(0f);
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startAnchoredPosition;
            rectTransform.localScale = startScale;
            rectTransform.localRotation = startRotation;
        }

        if (targetGraphic != null)
        {
            targetGraphic.color = startColor;
        }

        SetEffectAlpha(0f);
    }

    private void Update()
    {
        hoverAmount = Mathf.MoveTowards(
            hoverAmount,
            isHovering ? 1f : 0f,
            Time.unscaledDeltaTime * hoverLerpSpeed
        );
        hoverPunch = Mathf.MoveTowards(hoverPunch, 0f, Time.unscaledDeltaTime * 4f);

        float floatX = Mathf.Sin((Time.unscaledTime * horizontalFloatSpeed) + horizontalOffset) * horizontalFloatAmplitude;
        float floatY = Mathf.Sin((Time.unscaledTime * verticalFloatSpeed) + verticalOffset) * verticalFloatAmplitude;
        float driftX = Mathf.Cos((Time.unscaledTime * verticalFloatSpeed * 0.55f) + verticalOffset) * horizontalFloatAmplitude * 0.35f;
        float lift = hoverLift * hoverAmount;
        Vector2 targetPosition = startAnchoredPosition + new Vector2(floatX + driftX, floatY + lift);
        Vector3 targetScale = startScale * Mathf.Lerp(1f, hoverScale + (hoverPunch * 0.08f), hoverAmount);
        float wobble = Mathf.Sin(Time.unscaledTime * hoverWobbleSpeed) * hoverWobbleAngle * hoverAmount;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.unscaledDeltaTime * hoverLerpSpeed
        );

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * hoverLerpSpeed
        );

        rectTransform.localRotation = startRotation * Quaternion.Euler(0f, 0f, wobble);

        if (targetGraphic != null)
        {
            Color targetColor = Color.Lerp(startColor, GetBrightenedColor(), hoverAmount);
            targetGraphic.color = Color.Lerp(
                targetGraphic.color,
                targetColor,
                Time.unscaledDeltaTime * hoverLerpSpeed
            );
        }

        SetEffectAlpha(hoverAmount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverPunch = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private Color GetBrightenedColor()
    {
        Color brightened = startColor * hoverBrightness;
        brightened.a = startColor.a;

        return Color.Lerp(brightened, hoverTint, 0.35f);
    }

    private void SetEffectAlpha(float alpha)
    {
        if (hoverOutline != null)
        {
            Color color = glowColor;
            color.a *= alpha;
            hoverOutline.effectColor = color;
            hoverOutline.effectDistance = glowDistance * Mathf.Lerp(0.4f, 1f, alpha);
            hoverOutline.enabled = alpha > 0.01f;
        }

        if (hoverShadow != null)
        {
            Color color = shadowColor;
            color.a *= alpha;
            hoverShadow.effectColor = color;
            hoverShadow.effectDistance = shadowDistance * Mathf.Lerp(0.4f, 1f, alpha);
            hoverShadow.enabled = alpha > 0.01f;
        }
    }

    private Shadow GetShadowComponent()
    {
        Shadow[] shadows = GetComponents<Shadow>();
        for (int i = 0; i < shadows.Length; i++)
        {
            if (shadows[i] is Outline)
            {
                continue;
            }

            return shadows[i];
        }

        return null;
    }
}
