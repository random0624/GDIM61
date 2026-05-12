using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private List<TextMeshProUGUI> _branchTexts;
    [SerializeField] private List<Button> _branchButtons;
    [SerializeField] private float typeSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isChoosed = false;

    void Start()
    {
        dialogPanel.SetActive(false);
        for (int i = 0; i < _branchButtons.Count; i++)
            _branchButtons[i].gameObject.SetActive(false);
        // 订阅事件
        DialogueManager.Instance.OnLineStarted += ShowLine;//收到通知展示UI
        DialogueManager.Instance.OnDialogueEnded += HideUI;//收到通知关闭展示
        DialogueManager.Instance.OnDialogueReplied += ShowChoice;
    }

    private void Update()
    {
        if (isChoosed)
        {
            for (int i = 0; i < _branchButtons.Count; i++)
            {
                _branchButtons[i].gameObject.SetActive(false);
                isChoosed = false;
            }
            //选择完关闭掉所有的按钮
        }
    }


    private void ShowLine(DialogueLine line)//展示对应line的文字和name
    {
        dialogPanel.SetActive(true);
        _nameText.text = line.speakerName;
        ShowText(line.content);
        _portraitImage.sprite = line.portrait;

        for (int i = 0; i < _branchButtons.Count; i++)
        {
            _branchButtons[i].gameObject.SetActive(false);
        }
    }

    public void ShowText(string content)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(content));
    }

    private IEnumerator TypeText(string content)
    {
        _contentText.text = "";
        foreach (char c in content)
        {
            _contentText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        typingCoroutine = null;
    }

    private void ShowChoice(DialogueLine line)
    {
        for (int i = 0; i < _branchButtons.Count; i++)//遍历所有按钮
        {
            if (i < line.replyOptions.Count)
            {
                _branchButtons[i].gameObject.SetActive(true);//打开
                _branchTexts[i].text = line.replyOptions[i];

                int index = i; // 防止闭包问题

                _branchButtons[i].onClick.RemoveAllListeners();
                _branchButtons[i].onClick.AddListener(() =>
                {
                    DialogueManager.Instance.SelectReply(index);
                    isChoosed = true;
                    //每个按钮附上监听
                });
            }
            else
            {
                _branchButtons[i].gameObject.SetActive(false);
            }
        }
    }


    private void HideUI()
    {
        dialogPanel.SetActive(false);

        for (int i = 0; i < _branchButtons.Count; i++)
        {
            _branchButtons[i].gameObject.SetActive(false);
        }
    }

    // 玩家点击对话框推进下一句
    public void OnClickNext()
    {
        if (DialogueManager.Instance.IsWaitingLoopChoice)
        {
            DialogueManager.Instance.DisplayNextLine();
            return;
        }

        for (int i = 0; i < _branchButtons.Count; i++)
        {
            if (_branchButtons[i].gameObject.activeSelf)
                return;
        }

        DialogueManager.Instance.DisplayNextLine();
    }
}
