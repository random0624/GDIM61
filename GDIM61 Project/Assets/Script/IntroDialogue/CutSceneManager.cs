using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private Image blackPanel;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] Button nextButton;
    [SerializeField] Button moveButton;
    private bool clicked;
    [SerializeField] private Image endingImage;
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private DialogueData startDialogue;

    [SerializeField] private float typeSpeed = 0.04f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        blackPanel.gameObject.SetActive(true);
        endingImage.gameObject.SetActive(false);
        moveButton.gameObject.SetActive(false);
        introText.text = "";
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
        StartCoroutine(PlayCutScene());
    }

    private IEnumerator PlayCutScene()
    {
        yield return new WaitForSeconds(1f);

        foreach (DialogueLine line in introDialogue.lines)
        {
            yield return StartCoroutine(TypeText(line.content));

            clicked = false;
            yield return new WaitUntil(() => clicked);
        }
        introText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        yield return StartCoroutine(Fade());
        DialogueManager.Instance.StartDialogue(startDialogue);
    }

    private IEnumerator Fade()
    {
        float time = 0f;

        Color color = blackPanel.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            blackPanel.color = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            yield return null;
        }

        blackPanel.color = new Color(
            color.r,
            color.g,
            color.b,
            0f
        );

        blackPanel.gameObject.SetActive(false);
    }
    private IEnumerator TypeText(string content)
    {
        introText.text = "";

        foreach (char c in content)
        {
            introText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void OnClick()
    {
        clicked = true;
    }
    private void OnDialogueEnded()
    {
        endingImage.gameObject.SetActive(true);
        moveButton.gameObject.SetActive(true);
    }

    public void MoveToScene()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
