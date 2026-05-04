using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour{
    public static QuestUI Instance { get; private set; }

    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;
    public Quest testQuest;
    public int testQuestAmount;
    private List<QuestProgress> testQuests = new();
    private readonly List<ObjectiveProgressDelta> progressDeltaBuffer = new();
    private readonly HashSet<(string questId, string objectiveId)> pendingObjectivePops = new();
    //Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnStateChanged += HandleStateChanged;
        }
    }

    public void Start(){
        for(int i = 0; i < testQuestAmount; i++){
            testQuests.Add(new QuestProgress(testQuest));
        }
        SyncQuestUIVisibility();
        UpdateQuestUI();
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnStateChanged -= HandleStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void HandleStateChanged(GameController.GameState state)
    {
        SetQuestUIVisible(state == GameController.GameState.Sailing);
    }

    private void SyncQuestUIVisibility()
    {
        if (GameController.Instance == null)
        {
            SetQuestUIVisible(false);
            return;
        }

        SetQuestUIVisible(GameController.Instance.CurrentState == GameController.GameState.Sailing);
    }

    private void SetQuestUIVisible(bool visible)
    {
        if (questListContent != null)
        {
            questListContent.gameObject.SetActive(visible);
        }
    }

    public void ReportObjectiveProgress(string objectiveID, int amount = 1)
    {
        progressDeltaBuffer.Clear();

        foreach (var quest in testQuests)
        {
            quest.ReportObjectiveProgress(objectiveID, amount, progressDeltaBuffer);
        }

        if (progressDeltaBuffer.Count == 0)
        {
            return;
        }

        pendingObjectivePops.Clear();
        foreach (var delta in progressDeltaBuffer)
        {
            if (delta.NewAmount > delta.PreviousAmount &&
                !string.IsNullOrEmpty(delta.QuestID) &&
                !string.IsNullOrEmpty(delta.ObjectiveID))
            {
                pendingObjectivePops.Add((delta.QuestID, delta.ObjectiveID));
            }
        }

        UpdateQuestUI();
    }

    public void UpdateQuestUI(){
        //Destroy existing quest entries
        foreach(Transform child in questListContent){
            Destroy(child.gameObject);
        }
        //Create new quest entries
        foreach(var quest in testQuests){
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.questName;

            foreach(var objective in quest.objectives){
                GameObject objTEXTGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTEXTGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; //Collect Treasure chest amt...

                TMPCollectPopFeedback popFeedback = objTEXTGO.GetComponent<TMPCollectPopFeedback>();
                if (popFeedback != null &&
                    pendingObjectivePops.Contains((quest.QuestID, objective.objectiveID)))
                {
                    popFeedback.PlayPop();
                }
            }
        }

        pendingObjectivePops.Clear();
    }
}

