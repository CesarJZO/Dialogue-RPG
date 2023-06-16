using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string conversantName;
        [SerializeField] private Dialogue dialogue;

        public string ConversantName => conversantName;

        public CursorType GetCursorType() => CursorType.Dialogue;

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!dialogue) return false;
            if (!Input.GetMouseButtonDown(0)) return false;
            if (!callingController.TryGetComponent(out PlayerConversant conversant)) return false;
            conversant.StartDialogue(this, dialogue);
            return true;
        }
    }
}
