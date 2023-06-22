using GameDevTV.Core.UI.Tooltips;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            Quest quest = GetComponent<QuestItemUI>().Quest;
            tooltip.GetComponent<QuestTooltipUI>().Setup(quest);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}
