using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;//���ɵ���
    public event Action<DialogueLine> OnLineStarted; // ���µ�һ�л���ʼ
    public event Action OnDialogueEnded;            // ���Ի�����
    public event Action<DialogueLine> OnDialogueReplied;

    private Queue<DialogueLine> _lineQueue = new Queue<DialogueLine>();//����˳��
    private DialogueLine _currentLine;
    private DialogueLine _returnLine;
    private bool _waitingLoopChoice;
    public bool IsWaitingLoopChoice => _waitingLoopChoice;
    public bool IsInDialogue { get; private set; }

    void Awake() => Instance = this;

    //�Ի���ʼ
    public void StartDialogue(DialogueData data)
    {
        if (data == null) return;
        _lineQueue.Clear();//ɾ���ϴε�line
        foreach (var line in data.lines) _lineQueue.Enqueue(line);//��SO�ĶԻ����Ž�ȥ
        IsInDialogue = true;//����Ի�true
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (_waitingLoopChoice)
        {
            _returnLine = null;
            EndDialogue();
            return;
        }
        if (_lineQueue.Count == 0)
        {
            if (_returnLine != null)
            {
                _currentLine = _returnLine;
                _waitingLoopChoice = true;
                OnLineStarted?.Invoke(_currentLine);
                ShowReplyOption();
                return;
            }

            EndDialogue();
            return;
        }
        _currentLine = _lineQueue.Dequeue();// ��ȡ����ǰ��
        OnLineStarted?.Invoke(_currentLine);//չʾ���

        if (_currentLine.replyOptions != null && _currentLine.replyOptions.Count > 0)//��������û��branch
        {
            if (_currentLine.loopQuestion)//�����loop����
            {
                _returnLine = _currentLine; //��¼���
                _waitingLoopChoice = true;
            }
            else
            {
                _waitingLoopChoice = false;
            }
            OnDialogueReplied?.Invoke(_currentLine);
        }
        else
        {
            _waitingLoopChoice = false;
        }
    }

    private void EndDialogue()
    {
        _currentLine = null;
        _returnLine = null;
        IsInDialogue = false;
        OnDialogueEnded?.Invoke();//�¼�end֪ͨ
        Debug.Log("�Ի�����");
    }

    private void ShowReplyOption()
    {
        OnDialogueReplied?.Invoke(_currentLine);
    }
    public void SelectReply(int index)
    {
        if (_currentLine == null) return;
        if (_currentLine.replyBranches == null) return;
        if (index < 0 || index >= _currentLine.replyBranches.Count) return;

        DialogueData nextDialogue = _currentLine.replyBranches[index];

        _lineQueue.Clear();

        // û�к����Ի����Ž���
        if (nextDialogue == null)
        {
            if (_currentLine.loopQuestion)
            {
                _returnLine = null;
            }

            EndDialogue();
            return;
        }

        foreach (var line in nextDialogue.lines)
        {
            _lineQueue.Enqueue(line);
        }

        _waitingLoopChoice = false;
        DisplayNextLine();
    }
}