using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue currentDialogue;

        public string GetText() => !currentDialogue ? "No dialogue set" : currentDialogue.RootNode.Text;
    }
}
