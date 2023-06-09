using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public sealed class DialogueNode : ScriptableObject
    {
        [SerializeField, TextArea] private string text;
        private readonly List<string> _children = new();
        private Rect _rect = new(0f, 0f, 200f, 120f);

        public IEnumerable<string> Children => _children.AsReadOnly();

        public Rect Rect => _rect;

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
                _rect.position = value;
                EditorUtility.SetDirty(this);
            }
        }

        public void Initialize(string nodeName, Vector2 position)
        {
            name = nodeName;
            _rect.position = position;
        }

        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childId)
        {
            if (!_children.Contains(childId)) return;
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childId);
            EditorUtility.SetDirty(this);
        }
#endif

        public bool HasChild(string childId) => _children.Contains(childId);
    }
}
