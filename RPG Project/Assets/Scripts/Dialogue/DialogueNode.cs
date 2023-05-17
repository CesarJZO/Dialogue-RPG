using System;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        public string id;

        [TextArea] public string text;

        public string children;
    }
}
