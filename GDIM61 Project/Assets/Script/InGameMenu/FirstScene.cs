using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private Button start;
    [SerializeField] private Button quit;
    [SerializeField] private TextMeshProUGUI remindText;

    private void Start()
    {
        start.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
    }

    private void Update()
    {
        bool keyInput = Input.anyKeyDown;
        bool mouseInput = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (keyInput || mouseInput)
        {
            start.gameObject.SetActive(true);
            quit.gameObject.SetActive(true);
            remindText.gameObject.SetActive(false);
        }
    }

    public void StartButton()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
