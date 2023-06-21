using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private Quest[] quests;
        [SerializeField] private QuestItemUI questPrefab;

        private void Start()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }

            foreach (Quest quest in quests)
            {
                QuestItemUI questInstance = Instantiate(questPrefab, transform);
                questInstance.Setup(quest);
            }
        }
    }
}
