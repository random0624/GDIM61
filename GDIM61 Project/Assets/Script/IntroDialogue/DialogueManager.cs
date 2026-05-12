using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;//生成单例
    public event Action<DialogueLine> OnLineStarted; // 当新的一行话开始
    public event Action OnDialogueEnded;            // 当对话结束
    public event Action<DialogueLine> OnDialogueReplied;

    private Queue<DialogueLine> _lineQueue = new Queue<DialogueLine>();//创建顺序
    private DialogueLine _currentLine;
    private DialogueLine _returnLine;
    private bool _waitingLoopChoice;
    public bool IsWaitingLoopChoice => _waitingLoopChoice;
    public bool IsInDialogue { get; private set; }

    void Awake() => Instance = this;

    //对话开始
    public void StartDialogue(DialogueData data)
    {
        if (data == null) return;
        _lineQueue.Clear();//删除上次的line
        foreach (var line in data.lines) _lineQueue.Enqueue(line);//把SO的对话都放进去
        IsInDialogue = true;//进入对话true
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
        _currentLine = _lineQueue.Dequeue();// 先取出当前句
        OnLineStarted?.Invoke(_currentLine);//展示这句

        if (_currentLine.replyOptions != null && _currentLine.replyOptions.Count > 0)//如果这句中没有branch
        {
            if (_currentLine.loopQuestion)//如果是loop性质
            {
                _returnLine = _currentLine; //记录这段
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
        OnDialogueEnded?.Invoke();//事件end通知
        Debug.Log("对话结束");
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

        // 没有后续对话，才结束
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