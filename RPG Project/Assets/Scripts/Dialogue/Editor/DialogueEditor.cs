﻿using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public sealed class DialogueEditor : EditorWindow
    {
        private static Dialogue _selectedDialogueAsset;

        [NonSerialized] private DialogueNode _creatingNode;
        [NonSerialized] private DialogueNode _deletingNode;
        [NonSerialized] private DialogueNode _linkingParentNode;

        [NonSerialized] private DialogueNode _draggingNode;

        [NonSerialized] private Vector2 _draggingOffset;

        [NonSerialized] private GUIStyle _nodeStyle;

        /// <summary>
        ///     Opens the Dialogue Editor window when the menu item is clicked.
        /// </summary>
        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            GetWindow<DialogueEditor>(
                title: "Dialogue Editor",
                focus: true,
                desiredDockNextTo: typeof(SceneView)
            );
        }

        /// <summary>
        ///     Called when any asset is opened in the editor, then checks if it is a Dialogue asset and opens the Dialogue Editor
        ///     window.
        /// </summary>
        /// <param name="instanceID">Dialogue asset instance ID</param>
        /// <param name="line"></param>
        /// <returns>Whether it could be opened</returns>
        [OnOpenAsset(1)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            _selectedDialogueAsset = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (!_selectedDialogueAsset) return false;
            ShowWindow();
            return true;
        }

        private void OnGUI()
        {
            if (!_selectedDialogueAsset)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
                return;
            }

            ProcessEvents();

            foreach (DialogueNode node in _selectedDialogueAsset)
                DrawConnections(node);
            foreach (DialogueNode node in _selectedDialogueAsset)
                DrawNode(node);

            if (_creatingNode != null)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Add dialogue node");
                _selectedDialogueAsset.CreateNode(_creatingNode);
                _creatingNode = null;
            }

            if (_deletingNode != null)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Delete dialogue node");
                _selectedDialogueAsset.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        /// <summary>
        ///     Processes the mouse events for the Dialogue Editor window.
        /// </summary>
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null && Event.current.button == 0)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (_draggingNode != null)
                    _draggingOffset = Event.current.mousePosition - _draggingNode.rect.position;
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Move Dialogue Node");
                _draggingNode.rect.position = Event.current.mousePosition - _draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null && Event.current.button == 0)
            {
                _draggingNode = null;
            }
        }

        /// <summary>
        ///     Gets the foremost DialogueNode at the given point.
        /// </summary>
        /// <param name="currentMousePosition">The point to check</param>
        /// <returns></returns>
        private static DialogueNode GetNodeAtPoint(Vector2 currentMousePosition)
        {
            return _selectedDialogueAsset.LastOrDefault(node => node.rect.Contains(currentMousePosition));
        }

        /// <summary>
        ///     Draws the GUI for a DialogueNode.
        /// </summary>
        /// <param name="node">The DialogueNode to draw</param>
        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, _nodeStyle);

            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextArea(node.text, GUILayout.Height(50f));

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogueAsset, "Update dialogue text");
                node.text = newText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
                _creatingNode = node;

            DrawLinkButtons(node);

            if (GUILayout.Button("Delete"))
                _deletingNode = node;

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = node.rect.center + Vector2.right * node.rect.width / 2f;

            foreach (DialogueNode childNode in _selectedDialogueAsset.GetChildren(node))
            {
                var endPosition = new Vector3
                {
                    x = childNode.rect.xMin,
                    y = childNode.rect.center.y
                };

                Vector3 controlOffset = endPosition - startPosition;
                controlOffset.y = 0f;
                controlOffset.x *= 0.8f;

                Handles.DrawBezier(
                    startPosition,
                    endPosition,
                    startPosition + controlOffset,
                    endPosition - controlOffset,
                    Color.white,
                    null,
                    4f
                );
            }
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                    _linkingParentNode = node;
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                    _linkingParentNode = null;
            }
            else if (_linkingParentNode.HasChild(node.id))
            {
                if (GUILayout.Button("Unlink"))
                {
                    Undo.RecordObject(_selectedDialogueAsset, "Remove dialogue link");
                    _linkingParentNode.RemoveChild(node.id);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    Undo.RecordObject(_selectedDialogueAsset, "Add dialogue link");
                    _linkingParentNode.AddChild(node.id);
                    _linkingParentNode = null;
                }
            }
        }

        private void OnSelectionChanged()
        {
            var selected = Selection.activeObject as Dialogue;

            if (selected)
                _selectedDialogueAsset = selected;

            Repaint();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _nodeStyle = new GUIStyle
            {
                normal = { background = EditorGUIUtility.Load("node0") as Texture2D },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}
