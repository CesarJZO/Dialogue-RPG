using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private static Dialogue _selectedDialogueAsset;

        private GUIStyle _nodeStyle;

        private bool _dragging;

        /// <summary>
        /// Opens the Dialogue Editor window when the menu item is clicked.
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
        /// Called when any asset is opened in the editor, then checks if it is a Dialogue asset and opens the Dialogue Editor window.
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
            {
                OnGUINode(node);
            }
        }

        /// <summary>
        /// Processes the mouse events for the Dialogue Editor window.
        /// </summary>
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !_dragging && Event.current.button == 0)
            {
                _dragging = true;

            }
            else if (Event.current.type == EventType.MouseDrag && _dragging)
            {
                Undo.RecordObject(_selectedDialogueAsset, "Move Dialogue Node");
                _selectedDialogueAsset.RootNode.rect.position = Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _dragging && Event.current.button == 0)
            {
                _dragging = false;
            }

        }

        /// <summary>
        /// Draws the GUI for a DialogueNode.
        /// </summary>
        /// <param name="node">The DialogueNode to draw</param>
        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, _nodeStyle);
            EditorGUILayout.LabelField($"Node {node.id}", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            string newId = EditorGUILayout.TextField(node.id);
            string newText = EditorGUILayout.TextArea(node.text);

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                string message;
                if (newId != node.id)
                    message = nameof(node.id);
                else if (newText != node.text)
                    message = nameof(node.text);
                else
                    return;
                Undo.RecordObject(_selectedDialogueAsset, $"Update dialogue {message}");

                node.text = newText;
                node.id = newId;
            }

            GUILayout.EndArea();
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
