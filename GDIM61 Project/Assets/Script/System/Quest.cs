using UnityEngine;
using System.Collections.Generic;
using System;

public readonly struct ObjectiveProgressDelta
{
    public readonly string QuestID;
    public readonly string ObjectiveID;
    public readonly int PreviousAmount;
    public readonly int NewAmount;

    public ObjectiveProgressDelta(string questId, string objectiveId, int previousAmount, int newAmount)
    {
        QuestID = questId;
        ObjectiveID = objectiveId;
        PreviousAmount = previousAmount;
        NewAmount = newAmount;
    }
}

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string questDescription;
    public List<QuestObjective> objectives;

//called when the quest is enabled
    private void OnValidate(){
        if(string.IsNullOrEmpty(questID)){
        questID = questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]

    public class QuestObjective{
        public string objectiveID; //Match with itemID that you need to collect...
        public string description;
        public ObjectiveType type;
        public int currentAmount;
        public int requiredAmount;

        public bool IsCompleted => currentAmount >= requiredAmount;
    }

//Types of objectives
    public enum ObjectiveType{
        CollectItem,
        ReachLocation
    }

    [System.Serializable]

    public class QuestProgress{
        public Quest quest;
        public List<QuestObjective> objectives;
        public QuestProgress(Quest quest){
            this.quest = quest;
            objectives = new List<QuestObjective>();

            //Deep copy avoid modifying original
            foreach(var objective in quest.objectives){
                objectives.Add(new QuestObjective{
                    objectiveID = objective.objectiveID,
                    description = objective.description,
                    type = objective.type,
                    currentAmount = 0,
                    requiredAmount = objective.requiredAmount
                });
            }
        }
        public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);
        public string QuestID => quest.questID;

        public bool ReportObjectiveProgress(
            string objectiveID,
            int amount = 1,
            List<ObjectiveProgressDelta> deltasOut = null)
        {
            if (string.IsNullOrEmpty(objectiveID) || amount <= 0)
            {
                return false;
            }

            bool changed = false;
            string questId = quest != null ? quest.questID : string.Empty;

            foreach (var objective in objectives)
            {
                if (objective.objectiveID != objectiveID || objective.IsCompleted)
                {
                    continue;
                }

                int before = objective.currentAmount;
                objective.currentAmount = Mathf.Clamp(
                    objective.currentAmount + amount,
                    0,
                    objective.requiredAmount
                );

                if (objective.currentAmount != before)
                {
                    changed = true;
                    deltasOut?.Add(
                        new ObjectiveProgressDelta(
                            questId,
                            objective.objectiveID,
                            before,
                            objective.currentAmount));
                }
            }

            return changed;
        }
    }