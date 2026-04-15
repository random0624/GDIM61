using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public Button sailButton;
    public Button paintButton;
    public Button quitButton;
    public RawImage DrawingCanvas;
    private void Start()
    {
        sailButton.gameObject.SetActive(true);
        paintButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        DrawingCanvas.gameObject.SetActive(false);

        sailButton.onClick.AddListener(() => OnSailButtonClicked());
        paintButton.onClick.AddListener(() => OnPaintButtonClicked());
        quitButton.onClick.AddListener(() => OnQuitButtonClicked());
    }

    private void OnSailButtonClicked()
    {
        sailButton.gameObject.SetActive(false);
        paintButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        GameController.Instance.StartSail();
    }
    private void OnPaintButtonClicked()
    {
        sailButton.gameObject.SetActive(false);
        paintButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        GameController.Instance.StartPaint();
        DrawingCanvas.gameObject.SetActive(true);

    }
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
