using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [SerializeField] private int currentMoney;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI inShopMoneyText;

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

    public void ReduceMoney(int amount)
    {
        if (amount > currentMoney)
            return;
        currentMoney -= amount;
        UpdateUI();

        Debug.Log("Money : " + currentMoney);

        SaveMoney();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + currentMoney;
            inShopMoneyText.text = "$ " + currentMoney;
        }
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("CurrentMoney", currentMoney);
        PlayerPrefs.Save();
    }
}