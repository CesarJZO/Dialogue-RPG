using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        [Serializable]
        public class Reward
        {
            [Min(1)] public int amount;
            public InventoryItem item;
        }

        [SerializeField] private Objective[] objectives;
        [SerializeField] private List<Reward> rewards = new();

        public string Title => name;
        public int ObjectiveCount => objectives.Length;
        public IEnumerable<Objective> Objectives => objectives;

        public IEnumerable<Reward> Rewards => rewards;

        public bool HasObjective(string objectiveReference)
        {
            return objectives.Any(objective => objective.reference == objectiveReference);
        }

        public static Quest GetByName(string questName)
        {
            return Resources.LoadAll<Quest>(string.Empty).FirstOrDefault(quest => quest.name == questName);
        }
    }
}
