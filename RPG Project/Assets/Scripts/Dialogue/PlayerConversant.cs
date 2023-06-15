using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        public event Action ConversationUpdated;

        private Dialogue _currentDialogue;

        private DialogueNode _currentNode;
        private bool _isChoosing;

        public void StartDialogue(Dialogue dialogue)
        {
            _currentDialogue = dialogue;
            _currentNode = _currentDialogue.RootNode;
            ConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            _currentDialogue = null;
            _currentNode = null;
            _isChoosing = false;
            ConversationUpdated?.Invoke();
        }

        public bool HasDialogue => _currentDialogue;

        public bool IsChoosing() => _isChoosing;

        /// <summary>
        ///     Returns the text of the current node.
        /// </summary>
        /// <returns>The text of the current node if available, otherwise <c>"Node not available"</c></returns>
        public string GetText() => !_currentNode ? "Node not available" : _currentNode.Text;

        /// <summary>
        ///     Returns the children of the current node.
        /// </summary>
        /// <returns>The children of the current node</returns>
        public IEnumerable<DialogueNode> GetChoices() => _currentDialogue.GetChildren(_currentNode);

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            _isChoosing = false;
            Next();
        }

        /// <summary>
        ///     Moves to the next node in the dialogue.
        /// </summary>
        public void Next()
        {
            int playerResponsesCount = _currentDialogue.GetPlayerChildren(_currentNode).Count();
            if (playerResponsesCount > 0)
            {
                _isChoosing = true;
                ConversationUpdated?.Invoke();
                return;
            }

            List<DialogueNode> children = _currentDialogue.GetAIChildren(_currentNode).ToList();
            _currentNode = children[Random.Range(0, children.Count)];
            ConversationUpdated?.Invoke();
        }

        /// <summary>
        ///     Returns true if the current node has any children.
        /// </summary>
        /// <returns>Whether the current node has any children</returns>
        public bool HasNext() => _currentDialogue.GetChildren(_currentNode).Any();
    }
}
