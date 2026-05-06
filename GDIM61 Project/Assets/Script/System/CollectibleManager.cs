using System;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    public event Action<int, int> OnCollectChanged;
    public event Action OnAllCollected;

    [SerializeField] private int totalCount = 1;
    [SerializeField] private int currentCount = 0;

    private bool hasTriggeredCompletion = false;

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
        if (!hasTriggeredCompletion && IsAllCollected)
        {
            hasTriggeredCompletion = true;
            OnAllCollected?.Invoke();
        }
    }

    public void ResetCollect()
    {
        currentCount = 0;
        hasTriggeredCompletion = false;
        OnCollectChanged?.Invoke(currentCount, totalCount);
    }
}