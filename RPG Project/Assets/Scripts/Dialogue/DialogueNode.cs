using System;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        public string id;
        [TextArea] public string text;
        public string children;
        public Rect rect;
    }
}
