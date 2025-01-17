﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Core;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        public event Action Updated;

        private readonly List<QuestStatus> _statuses = new();

        public IEnumerable<QuestStatus> Statuses => _statuses;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            var status = new QuestStatus(quest);
            _statuses.Add(status);
            Updated?.Invoke();
        }

        private bool HasQuest(Quest quest) => _statuses.Any(status => status.Quest == quest);

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }
            Updated?.Invoke();
        }

        private QuestStatus GetQuestStatus(Quest quest) => _statuses.FirstOrDefault(status => status.Quest == quest);

        public object CaptureState()
        {
            var state = new List<object>();
            foreach (QuestStatus status in _statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            if (state is not List<object> stateList) return;

            _statuses.Clear();

            foreach (object objectState in stateList)
            {
                _statuses.Add(new QuestStatus(objectState));
            }
        }

        private void GiveReward(Quest quest)
        {
            foreach (Quest.Reward reward in quest.Rewards)
            {
                var inventory = GetComponent<Inventory>();
                bool success = inventory.AddToFirstEmptySlot(reward.item, reward.amount);
                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.amount);
                }
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            return predicate switch
            {
                "HasQuest" => HasQuest(Quest.GetByName(parameters[0])),
                "CompletedQuest" => GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete(),
                _ => null
            };
        }
    }
}
