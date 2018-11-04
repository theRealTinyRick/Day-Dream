using System.Collections;
using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    public class PlayerEvade : MonoBehaviour
    {
        public static string combatRollAnimation = "Combat Roll";

        private PlayerStateManager playerStateManager;
        private PlayerController playerController;
        private Animator animator;

        private void Start()
        {
            playerStateManager = GetComponent<PlayerStateManager>();
            playerController = GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
        }

        public void CombatRoll(Vector3 lookDirection = new Vector3())
        {
            if(lookDirection != Vector3.zero)
            {
                Quaternion rotation  = Quaternion.LookRotation(lookDirection);
                playerController.SetPlayerRotation(rotation);
            }

            animator.Play(combatRollAnimation);
            EnterState();
        }

        public void Phase()
        {

        }

        private void EnterState()
        {
            playerStateManager.SetStateHard(PlayerState.Evading);
        }

        private void ExitState()
        {
            if(playerStateManager.CurrentState == PlayerState.Evading)
            {
                playerStateManager.ResetState();
            }
        }

        #region Animation Events
        public void EvadeExit()
        {
            ExitState();
        }
        #endregion
    }
}
