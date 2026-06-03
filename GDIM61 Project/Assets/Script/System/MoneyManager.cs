using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [SerializeField] private int currentMoney;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI inShopMoneyText;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentMoney = PlayerPrefs.GetInt("CurrentMoney", 0);
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;

        UpdateUI();

        Debug.Log("Money : " + currentMoney);
        SaveMoney();
    }

    public bool CanSpend(int amount)
    {
        return currentMoney >= amount;
    }

    public bool ReduceMoney(int amount)
    {
        if (amount > currentMoney)
            return false;
        currentMoney -= amount;
        UpdateUI();

        Debug.Log("Money : " + currentMoney);

        SaveMoney();
        return true;
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + currentMoney;
        }

        if (inShopMoneyText != null)
        {
            inShopMoneyText.text = "$ " + currentMoney;
        }
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("CurrentMoney", currentMoney);
        PlayerPrefs.Save();
    }
}
