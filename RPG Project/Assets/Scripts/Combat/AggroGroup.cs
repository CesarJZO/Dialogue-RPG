using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] fighters;
        [SerializeField] private bool activateOnStart;

        private void Start()
        {
            Activate(activateOnStart);
        }

        public void Activate(bool activate)
        {
            foreach (Fighter fighter in fighters)
            {
                if (fighter.TryGetComponent(out CombatTarget combatTarget))
                    combatTarget.enabled = activate;
                fighter.enabled = activate;
            }
        }
    }
}
