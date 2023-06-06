using System;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public sealed class DialogueNode
    {
        public string id;
        [TextArea] public string text;
        public string[] children;
        [HideInInspector] public Rect rect = new(0f, 0f, 200f, 100f);

        public static implicit operator bool(DialogueNode node) => node != null;
    }
}
