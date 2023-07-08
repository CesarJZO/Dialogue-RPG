using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        /// <summary>
        ///     Invoked when the conversation is updated.
        /// </summary>
        public event Action ConversationUpdated;

        [SerializeField] private string playerName;

        /// <summary>
        ///     Used to fetch the current conversant name and dialogue triggers.
        /// </summary>
        [SerializeField] private AIConversant currentConversant;

        private Dialogue _currentDialogue;

        private DialogueNode _currentNode;
        private bool _isChoosing;

        public void StartDialogue(AIConversant conversant, Dialogue dialogue)
        {
            currentConversant = conversant;
            _currentDialogue = dialogue;
            _currentNode = _currentDialogue.RootNode;
            TriggerEnterAction();
            ConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            _currentDialogue = null;
            TriggerExitAction();
            _currentNode = null;
            currentConversant = null;
            _isChoosing = false;
            ConversationUpdated?.Invoke();
        }

        public bool HasDialogue => _currentDialogue;

        public bool IsChoosing() => _isChoosing;

        public string GetCurrentConversantName() => _isChoosing ? playerName : currentConversant.ConversantName;

        /// <summary>
        ///     Returns the text of the current node.
        /// </summary>
        /// <returns>The text of the current node if available, otherwise <c>"Node not available"</c></returns>
        public string GetText() => !_currentNode ? "Node not available" : _currentNode.Text;

        /// <summary>
        ///     Returns the children of the current node.
        /// </summary>
        /// <returns>The children of the current node</returns>
        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(_currentDialogue.GetChildren(_currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            _isChoosing = false;
            Next();
        }

        /// <summary>
        ///     Moves to the next node in the dialogue.
        /// </summary>
        public void Next()
        {
            int playerResponsesCount = FilterOnCondition(_currentDialogue.GetPlayerChildren(_currentNode)).Count();
            if (playerResponsesCount > 0)
            {
                _isChoosing = true;
                TriggerExitAction();
                ConversationUpdated?.Invoke();
                return;
            }

            List<DialogueNode> children = FilterOnCondition(_currentDialogue.GetAIChildren(_currentNode)).ToList();
            TriggerExitAction();
            _currentNode = children[Random.Range(0, children.Count)];
            TriggerEnterAction();
            ConversationUpdated?.Invoke();
        }

        /// <summary>
        ///     Returns true if the current node has any children.
        /// </summary>
        /// <returns>Whether the current node has any children</returns>
        public bool HasNext()
        {
            return FilterOnCondition(_currentDialogue.GetChildren(_currentNode)).Any();
        }

        private void TriggerEnterAction()
        {
            if (_currentNode)
            {
                TriggerAction(_currentNode.OnEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (_currentNode)
            {
                TriggerAction(_currentNode.OnExitAction);
            }
        }

        private void TriggerAction(string action)
        {
            if (action is "" or null) return;

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes)
        {
            foreach (DialogueNode node in inputNodes)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }
    }
}
