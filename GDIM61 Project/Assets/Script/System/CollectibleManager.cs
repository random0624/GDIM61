using System;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    public event Action<int, int> OnCollectChanged;

    [SerializeField] private int totalCount = 1;
    private int currentCount = 0;

    public int CurrentCount => currentCount;
    public int TotalCount => totalCount;
    public bool IsAllCollected => currentCount >= totalCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        OnCollectChanged?.Invoke(currentCount, totalCount);
    }

    public void AddCollect()
    {
        currentCount++;
        currentCount = Mathf.Clamp(currentCount, 0, totalCount);

        OnCollectChanged?.Invoke(currentCount, totalCount);
    }

    public void ResetCollect()
    {
        currentCount = 0;
        OnCollectChanged?.Invoke(currentCount, totalCount);
    }
}