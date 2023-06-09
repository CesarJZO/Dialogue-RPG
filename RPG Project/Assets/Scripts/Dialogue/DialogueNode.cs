using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public sealed class DialogueNode : ScriptableObject
    {
        [SerializeField, TextArea] private string text;

        private Rect _rect = new(0f, 0f, 200f, 120f);

        private readonly List<string> _children = new();

        public IEnumerable<string> Children => _children.Any() ? _children : Enumerable.Empty<string>();

        public string Text
        {
            set
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = value;
            }
            get => text;
        }

        public Rect Rect => new (_rect);

        public Vector2 Position
        {
            set
            {
                Undo.RecordObject(this, "Move Dialogue Node");
                _rect.position = value;
            }
            get => _rect.position;
        }

        public Vector2 Center => _rect.center;
        public Vector2 Top => new(_rect.center.x, _rect.yMin);
        public Vector2 TopLeft => _rect.min;
        public Vector2 TopRight => _rect.max;
        public Vector2 Bottom => new(_rect.center.x, _rect.yMax);
        public Vector2 BottomLeft => new(_rect.xMin, _rect.yMax);
        public Vector2 BottomRight => new(_rect.xMax, _rect.yMax);
        public Vector2 Left => new(_rect.xMin, _rect.center.y);
        public Vector2 Right => new(_rect.xMax, _rect.center.y);

        /// <summary>
        ///     Adds a child to the node.
        /// </summary>
        /// <param name="childId">The child node's Guid</param>
        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childId);
        }

        /// <summary>
        ///     Removes a child from the node.
        /// </summary>
        /// <param name="childId">The child node's Guid</param>
        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childId);
        }

        /// <summary>
        ///     Checks if the node has a child with the given Guid.
        /// </summary>
        /// <param name="childId">The child node's Guid</param>
        /// <returns>Whether or not the node has the child</returns>
        public bool HasChild(string childId) => _children.Contains(childId);
    }
}
