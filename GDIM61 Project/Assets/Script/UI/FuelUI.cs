using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] public Slider fuelSlider;
    private void Start()
    {
        if (BoatFuel.Instance != null)
        {
            // 先取消订阅防止重复订阅
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
