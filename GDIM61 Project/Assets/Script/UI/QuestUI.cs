using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;
    public Quest testQuest;
    public int testQuestAmount;
    private List<QuestProgress> testQuests = new();
    //Start is called before the first frame update

    public void Start(){
        for(int i = 0; i < testQuestAmount; i++){
            testQuests.Add(new QuestProgress(testQuest));
        }
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

            questNameText.text = quest.quest.name;

            foreach(var objective in quest.objectives){
                GameObject objTEXTGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTEXTGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; //Collect Treasure chest amt...
            }
        }
    }
}

