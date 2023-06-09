using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public sealed class Dialogue : ScriptableObject, IEnumerable<DialogueNode>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes;

        private readonly Dictionary<string, DialogueNode> _nodeLookup = new();

        public DialogueNode RootNode => nodes[0];

        private void OnValidate()
        {
            _nodeLookup.Clear();

            foreach (DialogueNode node in nodes)
                _nodeLookup[node.name] = node;
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
            if (parentNode.Children == null) yield break;

            foreach (string childId in parentNode.Children)
            {
                if (_nodeLookup.TryGetValue(childId, out DialogueNode value))
                    yield return value;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Creates a new node and adds it to the Dialogue and its parent node.
        /// </summary>
        /// <param name="parent">The parent node</param>
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Create Dialogue Node");
            Undo.RecordObject(this, "Add Dialogue Node");
            AddNode(newNode);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private static DialogueNode MakeNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.Initialize(
                Guid.NewGuid().ToString(),
                parent.Rect.position + Vector2.right * 250f
            );

            parent.AddChild(newNode.name);

            return newNode;
        }

        /// <summary>
        ///     Deletes a node and removes it from the Dialogue and its parent node.
        /// </summary>
        /// <param name="nodeToDelete">The node to delete</param>
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Delete Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren();
            Undo.DestroyObjectImmediate(nodeToDelete);

            void CleanDanglingChildren()
            {
                foreach (DialogueNode node in nodes)
                    node.RemoveChild(nodeToDelete.name);
            }
        }


#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            nodes ??= new List<DialogueNode>();
            if (nodes.Count == 0)
            {
                var newNode = CreateInstance<DialogueNode>();
                newNode.Initialize(Guid.NewGuid().ToString(), Vector2.zero);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) == string.Empty) return;
            foreach (DialogueNode node in nodes.Where(node => AssetDatabase.GetAssetPath(node) == string.Empty))
                AssetDatabase.AddObjectToAsset(node, this);
#endif
        }

        public void OnAfterDeserialize() { }
    }
}
