﻿using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string[] objectives;

        public string Title => name;
        public int ObjectiveCount => objectives.Length;
        public IEnumerable<string> Objectives => objectives;
    }
}