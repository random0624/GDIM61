using System;
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
        if (amount <= 0f)
        {
            return;
        }

        currentIntegrity -= amount;
        Debug.Log("Integrity: " + currentIntegrity);
        currentIntegrity = Mathf.Clamp(currentIntegrity, 0f, maxIntegrity);

        OnIntegrityChanged?.Invoke(currentIntegrity, maxIntegrity);
        BoatHitFeedback.PlayOnBoatDamage();
        CameraFollow.PlayHitShakeOnCamera();

        if (currentIntegrity <= 0f)
        {
            OnIntegrityEmpty?.Invoke();
        }

    }

    public void HealIntegrity()
    {
        currentIntegrity = maxIntegrity;
        OnIntegrityChanged?.Invoke(currentIntegrity, maxIntegrity);
        OnIntegrityFull?.Invoke();
    }

}
