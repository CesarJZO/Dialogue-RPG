using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public sealed class DialogueNode : ScriptableObject
    {
        [TextArea] public string text;
        public List<string> children = new();
        [HideInInspector] public Rect rect = new(0f, 0f, 200f, 120f);

        public void AddChild(string childId) => children.Add(childId);

        public void RemoveChild(string childId) => children.Remove(childId);

        public bool HasChild(string childId) => children.Contains(childId);
    }
}
