using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public sealed class DialogueEditor : EditorWindow
    {
        private const float CanvasSize = 4000f;
        private const float BackgroundSize = 50f;

        private static Dialogue _selectedDialogueAsset;
        private static readonly Rect BackgroundCoords = new(0f, 0f, CanvasSize / BackgroundSize, CanvasSize / BackgroundSize);

        [NonSerialized] private DialogueNode _creatingNode;
        [NonSerialized] private DialogueNode _deletingNode;
        [NonSerialized] private DialogueNode _linkingParentNode;

        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private Vector2 _draggingNodeOffset;

        [NonSerialized] private bool _draggingCanvas;
        [NonSerialized] private Vector2 _draggingCanvasOffset;

        [NonSerialized] private GUIStyle _nodeStyle;

        private Vector2 _scrollPosition;

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

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
            var backgroundTexture = Resources.Load<Texture2D>("background");
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, BackgroundCoords);

            foreach (DialogueNode node in _selectedDialogueAsset)
                DrawConnections(node);
            foreach (DialogueNode node in _selectedDialogueAsset)
                DrawNode(node);

            EditorGUILayout.EndScrollView();

            if (_creatingNode)
            {
                _selectedDialogueAsset.CreateNode(_creatingNode);
                _creatingNode = null;
            }

            if (_deletingNode)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Delete Dialogue Node");
                _selectedDialogueAsset.DeleteNode(_deletingNode);
                _deletingNode = null;
            }
        }

        /// <summary>
        ///     Processes the mouse events for the Dialogue Editor window.
        /// </summary>
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !_draggingNode && Event.current.button == 0)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode)
                {
                    _draggingNodeOffset = Event.current.mousePosition - _draggingNode.rect.position;

                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;

                    Selection.activeObject = _selectedDialogueAsset;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Move Dialogue Node");
                _draggingNode.rect.position = Event.current.mousePosition - _draggingNodeOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode && Event.current.button == 0)
            {
                _draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingCanvas && Event.current.button == 0)
            {
                _draggingCanvas = false;
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
                Undo.RecordObject(_selectedDialogueAsset, "Update Dialogue Text");
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
            if (!_linkingParentNode)
            {
                if (GUILayout.Button("Link"))
                    _linkingParentNode = node;
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                    _linkingParentNode = null;
            }
            else if (_linkingParentNode.HasChild(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    Undo.RecordObject(_selectedDialogueAsset, "Remove Dialogue Link");
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    Undo.RecordObject(_selectedDialogueAsset, "Add Dialogue Link");
                    _linkingParentNode.AddChild(node.name);
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

        private void OnUndoPerformed()
        {
            Repaint();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            Undo.undoRedoPerformed += OnUndoPerformed;

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
            Undo.undoRedoPerformed -= OnUndoPerformed;
        }
    }
}
