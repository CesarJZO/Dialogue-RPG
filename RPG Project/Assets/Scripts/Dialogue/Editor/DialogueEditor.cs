using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private static Dialogue _selectedDialogueAsset;

        [MenuItem("Window/Dialogue Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<DialogueEditor>("Dialogue Editor", focus: true, typeof(SceneView));
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

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(_selectedDialogueAsset.name);
            EditorGUILayout.EndVertical();
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {

        }
    }
}
