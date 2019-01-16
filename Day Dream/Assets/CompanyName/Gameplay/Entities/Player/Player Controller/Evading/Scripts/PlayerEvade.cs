using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerEvade : MonoBehaviour 
	{
		public const string EvadeAnimation = "Evade";
		public const string EvadeHorizontal = "EvadeHorizontal";
		public const string EvadeVertical = "EvadeVertical";

		private Animator animator;
		private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;
		private PlayerAttackAnimationController playerAttackAnimationController;
		private PlayerStateComponent playerStateComponent;

		public bool testLockOn = false;

		public bool isEvading;

        /// <summary>
        /// These are the states where the evade action is available
        /// </summary>
        [Tooltip("These are the states where the evade action is available")]
        public PlayerState[] availableStates;

		void Start () 
		{
			animator = GetComponent<Animator>();
			playerLocomotionAnimationHook = GetComponent<PlayerLocomotionAnimationHook>();
			playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
		}

		private void OnEnable() 
		{
			InputDriver.evadeButtonEvent.AddListener(Evade);
		}

		private void OnDisable() 
		{
			InputDriver.evadeButtonEvent.RemoveListener(Evade);	
		}
		
		private void Evade()
		{
            if(!CheckConditions())
            {
                return;
            }

			playerAttackAnimationController.StopAttacking();

			if(testLockOn)
			{
				if(InputDriver.LocomotionOrientationDirection != Vector3.zero)
				{
					animator.SetFloat(EvadeHorizontal, playerLocomotionAnimationHook.horizontalAnimatorFloat);
					animator.SetFloat(EvadeVertical, playerLocomotionAnimationHook.verticalAnimatorFloat);
					animator.Play(EvadeAnimation);
				}
			}
            else
            {
			    DefaultDash();
            }

            isEvading = true;
            playerStateComponent.SetStateHard(PlayerState.Evading);
		}

		private bool CheckConditions()
		{
            if(playerStateComponent)
            {
                if(!CheckState())
                {
					return false;
				}
            }

            return true;
		}

        private bool CheckState()
        {
            foreach(var _state in availableStates)
            {
                if(playerStateComponent.CheckState(_state))
                {
                    return true;
                }
            }

            return false;
        }

		private void DefaultDash()
		{
			animator.SetFloat(EvadeHorizontal, 0);
			animator.SetFloat(EvadeVertical, 0.5f);
			animator.Play(EvadeAnimation);
		}

		public void StoppedEvading()
		{
			isEvading = false;

            if(playerStateComponent.CurrentState == PlayerState.Evading)
            {
                playerStateComponent.ResetState();
            }
		}
	}
}
