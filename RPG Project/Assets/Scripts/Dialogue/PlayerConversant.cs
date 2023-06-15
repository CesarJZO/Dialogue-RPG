using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;
        private DialogueNode _currentNode;

        private bool _isChoosing;

        public bool IsChoosing()
        {
            return _isChoosing;
        }

        private void Awake()
        {
            if (!currentDialogue) return;
            _currentNode = currentDialogue.RootNode;
        }

        /// <summary>
        ///     Returns the text of the current node.
        /// </summary>
        /// <returns>The text of the current node if available, otherwise <c>"Node not available"</c></returns>
        public string GetText() => !_currentNode ? "Node not available" : _currentNode.Text;

        /// <summary>
        ///     Returns the children of the current node.
        /// </summary>
        /// <returns>The children of the current node</returns>
        public IEnumerable<DialogueNode> GetChoices() => currentDialogue.GetChildren(_currentNode);

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            _isChoosing = false;
            Next();
        }

        /// <summary>
        ///     Returns the children of the current node.
        /// </summary>
        public void Next()
        {
            int playerResponsesCount = currentDialogue.GetPlayerChildren(_currentNode).Count();
            if (playerResponsesCount > 0)
            {
                _isChoosing = true;
                return;
            }

            List<DialogueNode> children = currentDialogue.GetAIChildren(_currentNode).ToList();
            _currentNode = children[Random.Range(0, children.Count)];
        }

        /// <summary>
        ///     Returns true if the current node has any children.
        /// </summary>
        /// <returns>Whether the current node has any children</returns>
        public bool HasNext() => currentDialogue.GetChildren(_currentNode).Any();
    }
}
