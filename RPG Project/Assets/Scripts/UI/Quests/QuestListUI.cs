using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questPrefab;

        private QuestList _questList;

        private void Start()
        {
            _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            _questList.Updated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }

            foreach (QuestStatus questStatus in _questList.Statuses)
            {
                QuestItemUI questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(questStatus);
            }
        }
    }
}
