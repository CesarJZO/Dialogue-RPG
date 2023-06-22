using System;
using System.Collections.Generic;

namespace RPG.Quests
{
    public class QuestStatus
    {
        [Serializable]
        public class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives = new();
        }

        private readonly List<string> _completedObjectives = new();

        public QuestStatus(Quest quest)
        {
            Quest = quest;
        }

        public QuestStatus(object objectState)
        {
            if (objectState is not QuestStatusRecord state) return;
            Quest = Quest.GetByName(state.questName);
            _completedObjectives = state.completedObjectives;
        }

        public Quest Quest { get; }

        public int CompletedCount => _completedObjectives.Count;

        public bool IsComplete(string objective) => _completedObjectives.Contains(objective);

        public void CompleteObjective(string objective)
        {
            if (!Quest.HasObjective(objective)) return;
            if (IsComplete(objective)) return;
            _completedObjectives.Add(objective);
        }

        public object CaptureState()
        {
            return new QuestStatusRecord
            {
                questName = Quest.name,
                completedObjectives = _completedObjectives
            };
        }
    }
}
