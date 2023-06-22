using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [Serializable]
    public class QuestStatus
    {
        [SerializeField] private Quest quest;
        [SerializeField] private List<string> completedObjectives;

        public Quest Quest => quest;

        public int CompletedCount => completedObjectives.Count;

        public bool IsComplete(string objective) => completedObjectives.Contains(objective);
    }
}
