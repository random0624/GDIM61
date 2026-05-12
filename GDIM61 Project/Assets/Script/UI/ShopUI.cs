using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject chestPage;
    [SerializeField] private GameObject upgradePage;

    [Header("Tab Buttons")]
    [SerializeField] private Button chestTabButton;
    [SerializeField] private Button upgradeTabButton;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button upgradeHealthButton;
    [SerializeField] private Button upgradeFuelButton;

    [Header("OpenChest Button")]
    [SerializeField] private Button chestOpenButton;
    private void Start()
    {
        chestTabButton.onClick.AddListener(ShowChestPage);
        upgradeTabButton.onClick.AddListener(ShowUpgradePage);

        upgradeHealthButton.onClick.AddListener(() => OnHealthAddClicked());
        upgradeFuelButton.onClick.AddListener(() => OnFuelAddclicked());

        ShowChestPage();
        chestOpenButton.onClick.AddListener(OnChestOpenClicked); ;
    }

    private void ShowChestPage()
    {
        chestPage.SetActive(true);
        upgradePage.SetActive(false);
    }

    private void ShowUpgradePage()
    {
        chestPage.SetActive(false);
        upgradePage.SetActive(true);
    }

    private void OnChestOpenClicked()
    {
        if (CollectibleManager.Instance.CurrentChestCount != 0)
        {
            CollectibleManager.Instance.UseChest();
            MoneyManager.Instance.AddMoney(Random.Range(1,20));
        }
        else
            return;
    }

    private void OnHealthAddClicked()
    {
        BoatIntegrity.Instance.ChangeMaxIntergrity();
        MoneyManager.Instance.ReduceMoney(50);
    }
    
    private void OnFuelAddclicked()
    {
        BoatFuel.Instance.ChangeMaxFuel();
        MoneyManager.Instance.ReduceMoney(50);
    }
}
