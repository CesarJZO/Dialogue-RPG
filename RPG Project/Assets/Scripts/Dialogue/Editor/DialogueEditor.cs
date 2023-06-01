using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private static Dialogue _selectedDialogueAsset;

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

            foreach (DialogueNode node in _selectedDialogueAsset)
            {
                EditorGUILayout.LabelField($"Node {node.id}");

                EditorGUI.BeginChangeCheck();

                string newId = EditorGUILayout.TextField(node.id);
                string newText = EditorGUILayout.TextArea(node.text);

                EditorGUILayout.Space();

                if (!EditorGUI.EndChangeCheck()) continue;

                // Check whether the newId or new text has changed and depending on that, print a message of what has changed.
                string message;
                if (newId != node.id)
                    message = nameof(node.id);
                else if (newText != node.text)
                    message = nameof(node.text);
                else
                    continue;
                Undo.RecordObject(_selectedDialogueAsset, $"Update dialogue {message}");

                node.text = newText;
                node.id = newId;
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
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}
