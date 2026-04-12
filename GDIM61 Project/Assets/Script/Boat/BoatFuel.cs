using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatFuel : MonoBehaviour
{
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float currentFuel = 100f;

    public float MaxFuel => maxFuel;
    public float MaxFuel => maxFuel;
    public float CurrentFuel => currentFuel;
    public bool HasFuel => currentFuel > 0f;

    public event Action<float, float> OnFuelChanged;
    public event Action OnFuelEmpty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
