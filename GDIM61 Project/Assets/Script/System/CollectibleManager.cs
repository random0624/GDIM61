using System;
using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    // currentChest , maxChest
    public event Action<int, int> OnChestChanged;

    [SerializeField] private int maxChestCount = 99;
    [SerializeField] private int currentChestCount = 0;

    [SerializeField] private TextMeshProUGUI chestText;
    [SerializeField] private TextMeshProUGUI inshopchestText;

    public int CurrentChestCount => currentChestCount;
    public int MaxChestCount => maxChestCount;

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
        OnChestChanged?.Invoke(currentChestCount, maxChestCount);
    }

    // 加箱子
    public void AddChest(int amount = 1)
    {
        currentChestCount += amount;

        currentChestCount = Mathf.Clamp(
            currentChestCount,
            0,
            maxChestCount
        );

        OnChestChanged?.Invoke(currentChestCount, maxChestCount);
        UpdateUI();

        Debug.Log("Chest : " + currentChestCount);
    }

    // 消耗箱子
    public bool UseChest(int amount = 1)
    {
        if (currentChestCount < amount)
        {
            Debug.Log("Not Enough Chest");
            return false;
        }

        currentChestCount -= amount;

        currentChestCount = Mathf.Clamp(
            currentChestCount,
            0,
            maxChestCount
        );

        OnChestChanged?.Invoke(currentChestCount, maxChestCount);
        UpdateUI();

        return true;
    }

    // 重置
    public void ResetChest()
    {
        currentChestCount = 0;

        OnChestChanged?.Invoke(currentChestCount, maxChestCount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (chestText != null)
        {
            chestText.text = "Chest: " + currentChestCount;
            inshopchestText.text = "Chest: " + currentChestCount;
        }
    }
}