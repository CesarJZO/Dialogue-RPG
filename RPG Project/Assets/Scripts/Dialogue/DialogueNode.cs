using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public sealed class DialogueNode
    {
        public string id;
        [TextArea] public string text;
        public List<string> children;
        public Rect rect = new(0f, 0f, 200f, 120f);

        public DialogueNode()
        {
            id = Guid.NewGuid().ToString();
            children = new List<string>();
        }

        public void AddChild(string childId)
        {
            children.Add(childId);
        }
    }
}
