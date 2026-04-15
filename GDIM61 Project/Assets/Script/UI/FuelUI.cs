using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private Slider fuelSlider;
    private void Start()
    {
        if (BoatFuel.Instance != null)
        {
            BoatFuel.Instance.OnFuelChanged -= UpdateFuelBar;
            BoatFuel.Instance.OnFuelChanged += UpdateFuelBar;

            UpdateFuelBar(BoatFuel.Instance.currentFuel, BoatFuel.Instance.maxFuel);
        }
    }
    private void UpdateFuelBar(float current, float max)
    {
        fuelSlider.maxValue = max;
        fuelSlider.value = current;
    }
}
