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
            {
                var rootNode = new DialogueNode();
                nodes.Add(rootNode);
            }
#endif

            OnValidate();
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();

            foreach (DialogueNode node in nodes)
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
            if (parentNode.children == null) yield break;

            foreach (string childId in parentNode.children)
            {
                if (_nodeLookup.TryGetValue(childId, out DialogueNode value))
                    yield return value;
            }
        }

        /// <summary>
        ///     Creates a new node and adds it to the Dialogue and its parent node.
        /// </summary>
        /// <param name="parent">The parent node</param>
        public void CreateNode(DialogueNode parent)
        {
            var newNode = new DialogueNode();
            newNode.rect.position = parent.rect.position + Vector2.right * 250f;
            parent.AddChild(newNode.id);
            nodes.Add(newNode);
            OnValidate();
        }

        /// <summary>
        ///     Deletes a node and removes it from the Dialogue and its parent node.
        /// </summary>
        /// <param name="nodeToDelete">The node to delete</param>
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in nodes)
            {
                node.children.Remove(nodeToDelete.id);
            }
        }
    }
}
