using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button sailButton;
    [SerializeField] private Button paintButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button closeCanvasButton;
    [SerializeField] private Button closeShopButton;


    [Header("Canvas")]
    [SerializeField] private RawImage drawingCanvas;

    [SerializeField] private GameObject shopPanel;

    private bool openedFromMainMenu;

    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnMainMenuStarted -= MainMenuDisplay;
            GameController.Instance.OnMainMenuStarted += MainMenuDisplay;
        }

        MainMenuDisplay();

        sailButton.onClick.AddListener(OnSailButtonClicked);
        paintButton.onClick.AddListener(OnPaintButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);
        closeCanvasButton.onClick.AddListener(OnCloseCanvasButtonClicked);
        closeShopButton.onClick.AddListener(OnCloseShopButtonClicked);

        AddFloatEffect(sailButton);
        AddFloatEffect(paintButton);
        AddFloatEffect(quitButton);
        AddFloatEffect(closeCanvasButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPaintFromSailing();
        }
    }

    private void OnSailButtonClicked()
    {
        MainMenuHide();

        if (GameController.Instance != null)
        {
            GameController.Instance.StartSail();
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
    }
    private void OpenPaintFromSailing()
    {
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
        drawingCanvas.gameObject.SetActive(true);
        closeCanvasButton.gameObject.SetActive(true);
    }

    private void OnCloseCanvasButtonClicked()
    {
        drawingCanvas.gameObject.SetActive(false);
        closeCanvasButton.gameObject.SetActive(false);

        if (openedFromMainMenu)
        {
            MainMenuDisplay();

            if (GameController.Instance != null)
            {
                GameController.Instance.StartMainMenu();
            }
        }
        // ´Ó Sailing ESC ´ò¿ª
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
        sailButton.gameObject.SetActive(true);
        paintButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        shopButton.gameObject.SetActive(true);

        drawingCanvas.gameObject.SetActive(false);
        closeCanvasButton.gameObject.SetActive(false);
        shopPanel.gameObject.SetActive(false);
    }

    private void MainMenuHide()
    {
        sailButton.gameObject.SetActive(false);
        paintButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        shopButton.gameObject.SetActive(false);
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
        shopPanel.SetActive(true);
        MainMenuHide();
    }
    private void OnCloseShopButtonClicked()
    {
        MainMenuDisplay();
    }
}
