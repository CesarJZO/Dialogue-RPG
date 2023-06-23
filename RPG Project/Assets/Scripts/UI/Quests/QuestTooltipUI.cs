using System.Text;
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
        [SerializeField] private TextMeshProUGUI rewardText;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.Quest;
            title.text = quest.Title;

            foreach (Transform t in objectiveContainer)
            {
                Destroy(t.gameObject);
            }

            foreach (Quest.Objective objective in quest.Objectives)
            {
                GameObject prefab = status.IsObjectiveComplete(objective.reference)
                    ? objectivePrefab
                    : objectiveIncompletePrefab;

                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                var objectiveTextUI = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();

                objectiveTextUI.text = objective.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            StringBuilder builder = new();
            foreach (Quest.Reward reward in quest.Rewards)
            {
                if (builder.Length > 0)
                    builder.Append(", ");

                builder.Append(reward.amount > 1
                    ? $"{reward.amount} {reward.item.GetDisplayName()}s"
                    : reward.item.GetDisplayName()
                );
            }

            if (builder.Length == 0)
                builder.Append("No reward");

            builder.Append(".");

            return builder.ToString();
        }
    }
}
