using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, IEnumerable<DialogueNode>
    {
        [SerializeField] private List<DialogueNode> nodes;

#if UNITY_EDITOR
        private void Awake()
        {
            nodes ??= new List<DialogueNode>();

            if (nodes.Count == 0)
                nodes.Add(new DialogueNode());
        }
#endif

        public IEnumerator<DialogueNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
