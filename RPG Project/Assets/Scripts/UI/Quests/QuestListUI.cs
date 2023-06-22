using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questPrefab;

        private void Start()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }

            var questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();

            foreach (QuestStatus questStatus in questList.Statuses)
            {
                QuestItemUI questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(questStatus);
            }
        }
    }
}
