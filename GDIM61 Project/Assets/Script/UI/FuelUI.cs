using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private float smoothDuration = 0.18f;
    [SerializeField] private float lowFuelPercent = 0.25f;
    [SerializeField] private float lowPulseScale = 1.05f;
    [SerializeField] private float lowPulseSpeed = 4f;

    private Vector3 originalScale = Vector3.one;
    private bool hasInitializedSlider;
    private float targetFuelValue;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        TrySubscribeToFuel();
    }

    private void Start()
    {
        TrySubscribeToFuel();

        if (BoatFuel.Instance != null)
        {
            UpdateFuelBar(BoatFuel.Instance.currentFuel, BoatFuel.Instance.maxFuel);
        }
    }

    private void TrySubscribeToFuel()
    {
        if (BoatFuel.Instance != null)
        {
            BoatFuel.Instance.OnFuelChanged -= UpdateFuelBar;
            BoatFuel.Instance.OnFuelChanged += UpdateFuelBar;
        }
    }

    private void OnDisable()
    {
        if (BoatFuel.Instance != null)
        {
            BoatFuel.Instance.OnFuelChanged -= UpdateFuelBar;
        }

        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (fuelSlider == null || fuelSlider.maxValue <= 0f)
        {
            return;
        }

        if (hasInitializedSlider)
        {
            float duration = Mathf.Max(0.01f, smoothDuration);
            fuelSlider.value = Mathf.MoveTowards(
                fuelSlider.value,
                targetFuelValue,
                fuelSlider.maxValue * Time.deltaTime / duration
            );
        }

        float fuelPercent = fuelSlider.value / fuelSlider.maxValue;
        if (fuelPercent <= lowFuelPercent)
        {
            float pulse = (Mathf.Sin(Time.time * lowPulseSpeed) + 1f) * 0.5f;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * lowPulseScale, pulse);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * lowPulseSpeed);
        }
    }

    private void UpdateFuelBar(float current, float max)
    {
        if (fuelSlider == null)
        {
            return;
        }

        fuelSlider.maxValue = max;

        if (!hasInitializedSlider)
        {
            fuelSlider.value = current;
            targetFuelValue = current;
            hasInitializedSlider = true;
            return;
        }

        targetFuelValue = current;
    }
}
