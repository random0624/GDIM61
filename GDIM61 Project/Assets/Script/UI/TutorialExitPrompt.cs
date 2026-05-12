using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialExitPrompt : MonoBehaviour
{/*
    [Header("Prompt UI")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Press Enter to enter next level";
    [SerializeField] private bool hideOnStart = true;
    [SerializeField] private float fadeInDuration = 0.8f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "Level1";

    private CanvasGroup promptCanvasGroup;
    private GameObject promptTextObject;
    private bool canEnterNextLevel;
    private Coroutine fadeRoutine;
    private Coroutine subscribeRoutine;
    private CollectibleManager collectibleManager;

    private void Awake()
    {
        if (promptText == null)
        {
            promptText = GetComponentInChildren<TMP_Text>(true);
        }

        if (promptText != null)
        {
            promptTextObject = promptText.gameObject;
            promptCanvasGroup = promptTextObject.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
            {
                promptCanvasGroup = promptTextObject.AddComponent<CanvasGroup>();
            }

            promptCanvasGroup.alpha = 0f;

            if (hideOnStart)
            {
                promptTextObject.SetActive(false);
            }
        }

        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }

    private void OnEnable()
    {
        subscribeRoutine = StartCoroutine(WaitAndSubscribe());
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null)
        {
            StopCoroutine(subscribeRoutine);
            subscribeRoutine = null;
        }

        if (collectibleManager != null)
        {
            collectibleManager.OnAllCollected -= HandleAllCollected;
            collectibleManager = null;
        }
    }

    private void Update()
    {
        if (!canEnterNextLevel || string.IsNullOrWhiteSpace(nextSceneName))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void HandleAllCollected()
    {
        if (canEnterNextLevel)
        {
            return;
        }

        canEnterNextLevel = true;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeInPrompt());
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (CollectibleManager.Instance == null)
        {
            yield return null;
        }

        collectibleManager = CollectibleManager.Instance;
        collectibleManager.OnAllCollected -= HandleAllCollected;
        collectibleManager.OnAllCollected += HandleAllCollected;

        if (collectibleManager.IsAllCollected)
        {
            HandleAllCollected();
        }
    }

    private IEnumerator FadeInPrompt()
    {
        if (promptTextObject == null || promptCanvasGroup == null)
        {
            yield break;
        }

        promptTextObject.SetActive(true);

        float duration = Mathf.Max(0.01f, fadeInDuration);
        float elapsed = 0f;
        promptCanvasGroup.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            promptCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        promptCanvasGroup.alpha = 1f;
        fadeRoutine = null;
    }

*/
}
