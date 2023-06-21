using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;

        public void Setup(Quest quest)
        {
            title.text = quest.Title;
            progress.text = $"0/{quest.ObjectiveCount}";
        }
    }
}
