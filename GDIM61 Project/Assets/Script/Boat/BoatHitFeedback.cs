using System.Collections;
using UnityEngine;

public class BoatHitFeedback : MonoBehaviour
{
    public static BoatHitFeedback Instance { get; private set; }

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakeStrength = 0.12f;

    [Header("Flash")]
    [SerializeField] private Color hitColor = new Color(1f, 0.15f, 0.08f, 1f);
    [SerializeField] private float flashDuration = 0.22f;

    private Renderer[] renderers;
    private MaterialColorState[] materialColors;
    private Coroutine feedbackRoutine;
    private Vector3 currentShakeOffset;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        Instance = this;
        CacheRenderers();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayHit()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            ClearShake();
            RestoreColors();
        }

        feedbackRoutine = StartCoroutine(PlayHitRoutine());
    }

    public static void PlayOnBoatDamage()
    {
        if (Instance != null)
        {
            Instance.PlayHit();
        }
    }

    private IEnumerator PlayHitRoutine()
    {
        float elapsed = 0f;
        float totalDuration = Mathf.Max(shakeDuration, flashDuration);

        while (elapsed < totalDuration)
        {
            float shakeT = shakeDuration > 0f ? Mathf.Clamp01(elapsed / shakeDuration) : 1f;
            float flashT = flashDuration > 0f ? Mathf.Clamp01(elapsed / flashDuration) : 1f;

            ApplyShake(1f - shakeT);
            ApplyFlash(1f - flashT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ClearShake();
        RestoreColors();
        feedbackRoutine = null;
    }

    private void ApplyShake(float intensity)
    {
        ClearShake();

        if (intensity <= 0f || shakeStrength <= 0f)
        {
            return;
        }

        Vector2 randomCircle = Random.insideUnitCircle * shakeStrength * intensity;
        currentShakeOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        transform.localPosition += currentShakeOffset;
    }

    private void ClearShake()
    {
        if (currentShakeOffset == Vector3.zero)
        {
            return;
        }

        transform.localPosition -= currentShakeOffset;
        currentShakeOffset = Vector3.zero;
    }

    private void ApplyFlash(float intensity)
    {
        for (int i = 0; i < materialColors.Length; i++)
        {
            MaterialColorState state = materialColors[i];
            if (state.Material == null)
            {
                continue;
            }

            Color color = Color.Lerp(state.OriginalColor, hitColor, intensity);
            state.Material.SetColor(state.ColorPropertyId, color);
        }
    }

    private void RestoreColors()
    {
        for (int i = 0; i < materialColors.Length; i++)
        {
            MaterialColorState state = materialColors[i];
            if (state.Material != null)
            {
                state.Material.SetColor(state.ColorPropertyId, state.OriginalColor);
            }
        }
    }

    private void CacheRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        int colorCount = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            colorCount += renderers[i].materials.Length;
        }

        materialColors = new MaterialColorState[colorCount];
        int index = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                Material material = materials[j];
                int colorPropertyId = GetColorPropertyId(material);
                if (colorPropertyId == 0)
                {
                    continue;
                }

                materialColors[index] = new MaterialColorState
                {
                    Material = material,
                    ColorPropertyId = colorPropertyId,
                    OriginalColor = material.GetColor(colorPropertyId)
                };

                index++;
            }
        }

        if (index < materialColors.Length)
        {
            System.Array.Resize(ref materialColors, index);
        }
    }

    private int GetColorPropertyId(Material material)
    {
        if (material != null && material.HasProperty(BaseColorId))
        {
            return BaseColorId;
        }

        if (material != null && material.HasProperty(ColorId))
        {
            return ColorId;
        }

        return 0;
    }

    private struct MaterialColorState
    {
        public Material Material;
        public int ColorPropertyId;
        public Color OriginalColor;
    }
}
