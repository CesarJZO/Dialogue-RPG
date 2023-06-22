using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;

        public void CompleteObjective()
        {
            var questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
            questList.CompleteObjective(quest, objective);
        }
    }
}
