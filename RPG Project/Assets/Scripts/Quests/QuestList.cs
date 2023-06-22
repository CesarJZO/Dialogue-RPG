using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        public event Action Updated;

        private readonly List<QuestStatus> _statuses = new();

        public IEnumerable<QuestStatus> Statuses => _statuses;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            var status = new QuestStatus(quest);
            _statuses.Add(status);
            Updated?.Invoke();
        }

        private bool HasQuest(Quest quest) => _statuses.Any(status => status.Quest == quest);

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            Updated?.Invoke();
        }

        private QuestStatus GetQuestStatus(Quest quest) => _statuses.FirstOrDefault(status => status.Quest == quest);
    }
}
