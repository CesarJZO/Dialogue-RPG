using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.Quest;
            title.text = quest.Title;

            foreach (Transform t in objectiveContainer)
            {
                Destroy(t.gameObject);
            }

            foreach (string objective in quest.Objectives)
            {
                GameObject prefab = status.IsComplete(objective) ? objectivePrefab : objectiveIncompletePrefab;
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective;
            }
        }
    }
}
