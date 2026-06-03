using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private const int UpgradeCost = 50;

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

    [Header("Tab Visuals")]
    [SerializeField] private Color selectedTabColor = new Color(1f, 0.86f, 0.38f, 1f);

    private bool listenersRegistered;
    private bool tabColorsCached;
    private Color chestNormalTabColor = Color.white;
    private Color upgradeNormalTabColor = Color.white;

    private void OnEnable()
    {
        RegisterListeners();
        CacheTabColors();
        ShowChestPage();
        RefreshButtonStates();
    }

    private void Start()
    {
        AddFloatEffect(chestTabButton);
        AddFloatEffect(upgradeTabButton);
        AddFloatEffect(upgradeHealthButton);
        AddFloatEffect(upgradeFuelButton);
        AddFloatEffect(chestOpenButton);
    }

    private void OnDisable()
    {
        UnregisterListeners();
    }

    private void RegisterListeners()
    {
        if (listenersRegistered)
        {
            return;
        }

        AddListener(chestTabButton, ShowChestPage);
        AddListener(upgradeTabButton, ShowUpgradePage);
        AddListener(upgradeHealthButton, OnHealthAddClicked);
        AddListener(upgradeFuelButton, OnFuelAddclicked);
        AddListener(chestOpenButton, OnChestOpenClicked);
        listenersRegistered = true;
    }

    private void UnregisterListeners()
    {
        if (!listenersRegistered)
        {
            return;
        }

        RemoveListener(chestTabButton, ShowChestPage);
        RemoveListener(upgradeTabButton, ShowUpgradePage);
        RemoveListener(upgradeHealthButton, OnHealthAddClicked);
        RemoveListener(upgradeFuelButton, OnFuelAddclicked);
        RemoveListener(chestOpenButton, OnChestOpenClicked);
        listenersRegistered = false;
    }

    private void ShowChestPage()
    {
        SetActive(chestPage, true);
        SetActive(upgradePage, false);
        SetTabSelected(chestTabButton, true);
        SetTabSelected(upgradeTabButton, false);
        RefreshButtonStates();
    }

    private void ShowUpgradePage()
    {
        SetActive(chestPage, false);
        SetActive(upgradePage, true);
        SetTabSelected(chestTabButton, false);
        SetTabSelected(upgradeTabButton, true);
        RefreshButtonStates();
    }

    private void OnChestOpenClicked()
    {
        if (CollectibleManager.Instance != null &&
            CollectibleManager.Instance.UseChest())
        {
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(Random.Range(1, 20));
            }
        }

        RefreshButtonStates();
    }

    private void OnHealthAddClicked()
    {
        if (MoneyManager.Instance == null ||
            BoatIntegrity.Instance == null ||
            !MoneyManager.Instance.ReduceMoney(UpgradeCost))
        {
            RefreshButtonStates();
            return;
        }

        BoatIntegrity.Instance.ChangeMaxIntergrity();
        RefreshButtonStates();
    }
    
    private void OnFuelAddclicked()
    {
        if (MoneyManager.Instance == null ||
            BoatFuel.Instance == null ||
            !MoneyManager.Instance.ReduceMoney(UpgradeCost))
        {
            RefreshButtonStates();
            return;
        }

        BoatFuel.Instance.ChangeMaxFuel();
        RefreshButtonStates();
    }

    private void RefreshButtonStates()
    {
        if (chestOpenButton != null)
        {
            chestOpenButton.interactable = CollectibleManager.Instance != null &&
                CollectibleManager.Instance.CurrentChestCount > 0;
        }

        bool canUpgrade = MoneyManager.Instance != null && MoneyManager.Instance.CanSpend(UpgradeCost);
        if (upgradeHealthButton != null)
        {
            upgradeHealthButton.interactable = canUpgrade && BoatIntegrity.Instance != null;
        }

        if (upgradeFuelButton != null)
        {
            upgradeFuelButton.interactable = canUpgrade && BoatFuel.Instance != null;
        }
    }

    private void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void RemoveListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveListener(action);
        }
    }

    private void SetActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private void SetTabSelected(Button button, bool selected)
    {
        if (button == null)
        {
            return;
        }

        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = selected ? selectedTabColor : GetNormalTabColor(button);
        }

        MenuButtonFloatEffect effect = button.GetComponent<MenuButtonFloatEffect>();
        if (effect != null)
        {
            effect.CacheBaseline();
        }
    }

    private void AddFloatEffect(Button button)
    {
        if (button != null &&
            button.GetComponent<MenuButtonFloatEffect>() == null)
        {
            button.gameObject.AddComponent<MenuButtonFloatEffect>();
        }
    }

    private void CacheTabColors()
    {
        if (tabColorsCached)
        {
            return;
        }

        Image chestImage = chestTabButton != null ? chestTabButton.GetComponent<Image>() : null;
        if (chestImage != null)
        {
            chestNormalTabColor = chestImage.color;
        }

        Image upgradeImage = upgradeTabButton != null ? upgradeTabButton.GetComponent<Image>() : null;
        if (upgradeImage != null)
        {
            upgradeNormalTabColor = upgradeImage.color;
        }

        tabColorsCached = true;
    }

    private Color GetNormalTabColor(Button button)
    {
        if (button == chestTabButton)
        {
            return chestNormalTabColor;
        }

        if (button == upgradeTabButton)
        {
            return upgradeNormalTabColor;
        }

        return Color.white;
    }
}
