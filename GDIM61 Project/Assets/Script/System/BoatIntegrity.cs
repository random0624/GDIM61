using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatIntegrity : MonoBehaviour
{
    public static BoatIntegrity Instance { get; set; }
    public float maxIntegrity = 100f;
    public float currentIntegrity = 100f;
    public event Action<float, float> OnIntegrityChanged;
    public event Action OnIntegrityEmpty;
    public event Action OnIntegrityFull;

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
        currentIntegrity = maxIntegrity;
        OnIntegrityChanged?.Invoke(currentIntegrity, maxIntegrity);
    }

    public void ConsumeIntegrity(float amount)
    {
        currentIntegrity -= amount;
        Debug.Log("Fuel: " + currentIntegrity);
        currentIntegrity = Mathf.Clamp(currentIntegrity, 0f, maxIntegrity);

        OnIntegrityChanged?.Invoke(currentIntegrity, maxIntegrity);

        if (currentIntegrity <= 0f)
        {
            OnIntegrityEmpty?.Invoke();
        }

    }

}
