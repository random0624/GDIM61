using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegrityUI : MonoBehaviour
{
    [SerializeField] private Slider integritySlider;
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
        integritySlider.maxValue = max;
        integritySlider.value = current;
    }
}
