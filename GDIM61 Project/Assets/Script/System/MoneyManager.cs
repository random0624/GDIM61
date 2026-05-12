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
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;

        UpdateUI();

        Debug.Log("Money : " + currentMoney);
    }

    public void ReduceMoney(int amount)
    {
        currentMoney -= amount;
        UpdateUI();

        Debug.Log("Money : " + currentMoney);
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + currentMoney;
            inShopMoneyText.text = "$ " + currentMoney;
        }
    }
}