using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public sealed class Dialogue : ScriptableObject, IEnumerable<DialogueNode>
    {
        [SerializeField] private List<DialogueNode> nodes;

        private readonly Dictionary<string, DialogueNode> _nodeLookup = new();

        public DialogueNode RootNode => nodes[0];

        private void Awake()
        {
#if UNITY_EDITOR
            nodes ??= new List<DialogueNode>();

            if (nodes.Count == 0)
                nodes.Add(new DialogueNode());
#endif

            OnValidate();
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (DialogueNode node in this)
                _nodeLookup[node.id] = node;
        }

        public IEnumerator<DialogueNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<DialogueNode> GetChildren(DialogueNode parentNode)
        {
            // return parentNode.children.Where(id => _nodeLookup.ContainsKey(id)).Select(id => _nodeLookup[id]);
            foreach (string childId in parentNode.children)
            {
                if (_nodeLookup.TryGetValue(childId, out DialogueNode value))
                    yield return value;
            }
        }
    }
}
