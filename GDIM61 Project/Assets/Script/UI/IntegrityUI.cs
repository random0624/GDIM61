using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegrityUI : MonoBehaviour
{
    [SerializeField] private Slider integritySlider;
    private void Start()
    {
        if (BoatIntegrity.Instance != null)
        {
            BoatIntegrity.Instance.OnIntegrityChanged -= UpdateFuelBar;
            BoatIntegrity.Instance.OnIntegrityChanged += UpdateFuelBar;

            UpdateFuelBar(BoatFuel.Instance.currentFuel, BoatFuel.Instance.maxFuel);
        }
    }
    private void UpdateFuelBar(float current, float max)
    {
        integritySlider.maxValue = max;
        integritySlider.value = current;
    }
}
