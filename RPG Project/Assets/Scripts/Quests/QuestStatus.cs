using System.Collections.Generic;

namespace RPG.Quests
{
    public class QuestStatus
    {
        private readonly List<string> _completedObjectives = new();

        public QuestStatus(Quest quest)
        {
            Quest = quest;
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
    }
}
