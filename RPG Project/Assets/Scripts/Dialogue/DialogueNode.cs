using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    /// <summary>
    ///     A node in a dialogue graph.
    /// </summary>
    public sealed class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking;
        [SerializeField, TextArea] private string text;
        [SerializeField] private List<string> children = new();
        [SerializeField] private Rect rect = new(0f, 0f, 200f, 120f);
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;

        [SerializeField] private Condition condition;

        public IEnumerable<string> Children => children.AsReadOnly();

        public Rect Rect => rect;

        public bool IsPlayerSpeaking
        {
            get => isPlayerSpeaking;
#if UNITY_EDITOR
            set
            {
                Undo.RecordObject(this, "Change Dialogue Speaker");
                isPlayerSpeaking = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public string Text
        {
            get => text;
#if UNITY_EDITOR
            set
            {
                if (value == text) return;
                Undo.RecordObject(this, "Update Dialogue Text");
                text = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

#if UNITY_EDITOR
        public Vector2 Position
        {
            set
            {
                Undo.RecordObject(this, "Move Dialogue Node");
                rect.position = value;
                EditorUtility.SetDirty(this);
            }
        }

        public void Initialize(string nodeName, Vector2 position, bool isPlayerSpeaking)
        {
            name = nodeName;
            rect.position = position;
            this.isPlayerSpeaking = isPlayerSpeaking;
        }

        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childId)
        {
            if (!children.Contains(childId)) return;
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childId);
            EditorUtility.SetDirty(this);
        }
#endif

        public bool HasChild(string childId) => children.Contains(childId);

        public string OnEnterAction => onEnterAction;

        public string OnExitAction => onExitAction;

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> predicateEvaluators)
        {
            return condition.Check(predicateEvaluators);
        }
    }
}
