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

        public void Setup(Quest quest)
        {
            title.text = quest.Title;

            foreach (Transform t in objectiveContainer)
            {
                Destroy(t.gameObject);
            }

            foreach (string objective in quest.Objectives)
            {
                GameObject objectiveInstance = Instantiate(objectivePrefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective;
            }
        }
    }
}
