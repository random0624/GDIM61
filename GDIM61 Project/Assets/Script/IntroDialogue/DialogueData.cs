using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite portrait;
    public AudioClip voiceClip;

    [TextArea(3, 10)]
    public string content;

    public List<string> replyOptions; // 为空 = 普通行
    public List<DialogueData> replyBranches;//SO嵌套
    public bool loopQuestion;//是否为loop性质
}
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;//每个SO都是一个list
} 