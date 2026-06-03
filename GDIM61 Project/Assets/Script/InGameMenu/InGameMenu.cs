using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button sailButton;
    [SerializeField] private Button paintButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button nextRegionButton;
    [SerializeField] private Button closeCanvasButton;
    [SerializeField] private Button closeShopButton;


    [Header("Canvas")]
    [SerializeField] private RawImage drawingCanvas;

    [SerializeField] private GameObject shopPanel;

    private bool openedFromMainMenu;
    private bool listenersRegistered;

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnMainMenuStarted -= MainMenuDisplay;
            GameController.Instance.OnMainMenuStarted += MainMenuDisplay;
        }

        RegisterButtonListeners();
    }

    private void Start()
    {
        if (nextRegionButton != null)
        {
            nextRegionButton.interactable = false;
        }

        MainMenuDisplay();

        AddFloatEffect(sailButton);
        AddFloatEffect(paintButton);
        AddFloatEffect(quitButton);
        AddFloatEffect(shopButton);
        AddFloatEffect(nextRegionButton);
        AddFloatEffect(closeCanvasButton);
        AddFloatEffect(closeShopButton);
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnMainMenuStarted -= MainMenuDisplay;
        }

        UnregisterButtonListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPaintFromSailing();
        }

        if (Sticker.Instance == null)
            return;

        if (Sticker.Instance.isWin)
        {
            if (nextRegionButton != null && !nextRegionButton.interactable)
            {
                nextRegionButton.interactable = true;
                Debug.Log("Win");
            }
        }
    }

    private void OnSailButtonClicked()
    {
        MainMenuHide();

        if (GameController.Instance != null)
        {
            GameController.Instance.StartSail();
        }

        if (BoatFuel.Instance != null)
        {
            BoatFuel.Instance.Refill();
        }

        if (BoatIntegrity.Instance != null)
        {
            BoatIntegrity.Instance.HealIntegrity();
        }
    }

    private void OnPaintButtonClicked()
    {
        openedFromMainMenu = true;

        MainMenuHide();

        if (GameController.Instance != null)
        {
            GameController.Instance.StartPaint();
        }

        OpenCanvas();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
        ResetSaveData();
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    private void OpenPaintFromSailing()
    {
        if (drawingCanvas == null)
        {
            return;
        }

        if (drawingCanvas.gameObject.activeSelf)
            return;

        openedFromMainMenu = false;

        MainMenuHide();

        if (GameController.Instance != null)
        {
            GameController.Instance.StartPaint();
        }

        OpenCanvas();
    }

    private void OpenCanvas()
    {
        SetObjectActive(drawingCanvas != null ? drawingCanvas.gameObject : null, true);
        SetButtonActive(closeCanvasButton, true);
    }

    private void OnCloseCanvasButtonClicked()
    {
        SetObjectActive(drawingCanvas != null ? drawingCanvas.gameObject : null, false);
        SetButtonActive(closeCanvasButton, false);

        if (openedFromMainMenu)
        {
            MainMenuDisplay();

            if (GameController.Instance != null)
            {
                GameController.Instance.StartMainMenu();
            }
        }
        else
        {
            MainMenuHide();

            if (GameController.Instance != null)
            {
                GameController.Instance.StartSail();
            }
        }
    }

    private void MainMenuDisplay()
    {
        SetButtonActive(sailButton, true);
        SetButtonActive(paintButton, true);
        SetButtonActive(quitButton, true);
        SetButtonActive(shopButton, true);
        SetButtonActive(nextRegionButton, true);
        SetObjectActive(drawingCanvas != null ? drawingCanvas.gameObject : null, false);
        SetButtonActive(closeCanvasButton, false);
        SetObjectActive(shopPanel, false);
    }

    private void MainMenuHide()
    {
        SetButtonActive(sailButton, false);
        SetButtonActive(paintButton, false);
        SetButtonActive(quitButton, false);
        SetButtonActive(shopButton, false);
        SetButtonActive(nextRegionButton, false);
    }

    private void AddFloatEffect(Button button)
    {
        if (button != null &&
            button.GetComponent<MenuButtonFloatEffect>() == null)
        {
            button.gameObject.AddComponent<MenuButtonFloatEffect>();
        }
    }

    private void OnShopButtonClicked()
    {
        SetObjectActive(shopPanel, true);
        MainMenuHide();
    }
    private void OnCloseShopButtonClicked()
    {
        MainMenuDisplay();
    }

    private void NextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    private void RegisterButtonListeners()
    {
        if (listenersRegistered)
        {
            return;
        }

        AddListener(sailButton, OnSailButtonClicked);
        AddListener(paintButton, OnPaintButtonClicked);
        AddListener(quitButton, OnQuitButtonClicked);
        AddListener(shopButton, OnShopButtonClicked);
        AddListener(closeCanvasButton, OnCloseCanvasButtonClicked);
        AddListener(closeShopButton, OnCloseShopButtonClicked);
        AddListener(nextRegionButton, NextLevel);
        listenersRegistered = true;
    }

    private void UnregisterButtonListeners()
    {
        if (!listenersRegistered)
        {
            return;
        }

        RemoveListener(sailButton, OnSailButtonClicked);
        RemoveListener(paintButton, OnPaintButtonClicked);
        RemoveListener(quitButton, OnQuitButtonClicked);
        RemoveListener(shopButton, OnShopButtonClicked);
        RemoveListener(closeCanvasButton, OnCloseCanvasButtonClicked);
        RemoveListener(closeShopButton, OnCloseShopButtonClicked);
        RemoveListener(nextRegionButton, NextLevel);
        listenersRegistered = false;
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

    private void SetButtonActive(Button button, bool active)
    {
        if (button != null)
        {
            button.gameObject.SetActive(active);
        }
    }

    private void SetObjectActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}
