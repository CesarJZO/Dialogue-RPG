using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, IEnumerable<DialogueNode>
    {
        [SerializeField] private List<DialogueNode> nodes;

        public DialogueNode RootNode => nodes[0];

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

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            return from childID in parentNode.children
                where !string.IsNullOrEmpty(childID)
                select nodes.FirstOrDefault(node => node.id == childID);
        }
    }
}
