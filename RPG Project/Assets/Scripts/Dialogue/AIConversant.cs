using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue dialogue;

        public CursorType GetCursorType() => CursorType.Dialogue;

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!dialogue) return false;
            if (!Input.GetMouseButtonDown(0)) return false;
            if (!callingController.TryGetComponent(out PlayerConversant conversant)) return false;
            conversant.StartDialogue(dialogue);
            return true;
        }
    }
}
