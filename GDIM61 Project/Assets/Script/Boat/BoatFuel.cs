using System;
using UnityEngine;

public class BoatFuel : MonoBehaviour
{
    public static BoatFuel Instance { get; set; }
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public event Action<float, float> OnFuelChanged;
    public event Action OnFuelEmpty;
    public event Action OnFuelFull;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        currentFuel = maxFuel;
        OnFuelChanged?.Invoke(currentFuel, maxFuel);
    }

    public void ConsumeFuel(float amount)
    {
        currentFuel -= amount;
        Debug.Log("Fuel: " + currentFuel);
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        OnFuelChanged?.Invoke(currentFuel, maxFuel);

        if (currentFuel <= 0f)
        {
            OnFuelEmpty?.Invoke();
        }

    }

    public void Refill()
    {
        currentFuel = maxFuel;
        OnFuelChanged?.Invoke(currentFuel, maxFuel);
        OnFuelFull?.Invoke();
    }

    public bool HasFuel()
    {
        return currentFuel > 0f;
    }

    public float GetFuelPercent()
    {
        return currentFuel / maxFuel;
    }
}
