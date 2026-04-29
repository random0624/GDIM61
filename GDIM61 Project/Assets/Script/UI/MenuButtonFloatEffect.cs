using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonFloatEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Float")]
    [SerializeField] private float bobAmplitude = 5f;
    [SerializeField] private float bobSpeed = 1.4f;

    [Header("Hover")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverLift = 6f;
    [SerializeField] private float hoverLerpSpeed = 10f;
    [SerializeField] private Color hoverTint = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private float hoverBrightness = 1.18f;

    private RectTransform rectTransform;
    private Graphic targetGraphic;
    private Vector2 startAnchoredPosition;
    private Vector3 startScale;
    private Color startColor = Color.white;
    private bool isHovering;
    private float bobOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetGraphic = GetComponent<Graphic>();

        if (targetGraphic == null)
        {
            targetGraphic = GetComponentInChildren<Graphic>();
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
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
        isHovering = false;

        if (targetGraphic != null)
        {
            startColor = targetGraphic.color;
        }
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startAnchoredPosition;
            rectTransform.localScale = startScale;
        }

        if (targetGraphic != null)
        {
            targetGraphic.color = startColor;
        }
    }

    private void Update()
    {
        float bob = Mathf.Sin((Time.unscaledTime * bobSpeed) + bobOffset) * bobAmplitude;
        float lift = isHovering ? hoverLift : 0f;
        Vector2 targetPosition = startAnchoredPosition + new Vector2(0f, bob + lift);
        Vector3 targetScale = startScale * (isHovering ? hoverScale : 1f);

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

        if (targetGraphic != null)
        {
            Color targetColor = isHovering ? GetBrightenedColor() : startColor;
            targetGraphic.color = Color.Lerp(
                targetGraphic.color,
                targetColor,
                Time.unscaledDeltaTime * hoverLerpSpeed
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private Color GetBrightenedColor()
    {
        Color brightened = startColor * hoverBrightness;
        brightened.a = startColor.a;

        return Color.Lerp(brightened, hoverTint, 0.25f);
    }
}
