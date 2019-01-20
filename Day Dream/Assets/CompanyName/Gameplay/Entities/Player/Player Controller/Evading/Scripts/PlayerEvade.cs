using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerEvade : MonoBehaviour 
	{
		public const string EvadeAnimation = "RollFront";
		///public const string EvadeAnimation = "Evade";
		public const string EvadeHorizontal = "EvadeHorizontal";
		public const string EvadeVertical = "EvadeVertical";

		private Animator animator;
		private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;
		private PlayerStateComponent playerStateComponent;

        /// <summary>
        /// TODO ADD SUMMARY
        /// </summary>
        [TabGroup(Tabs.Properties)]
        public bool isEvading;

        /// <summary>
        /// These are the states where the evade action is available
        /// </summary>
        [Tooltip("These are the states where the evade action is available")]
        [TabGroup(Tabs.Properties)]
        public PlayerState[] availableStates;

        [TabGroup(Tabs.Events)]
        public EvadeStartedEvents evadeStartedEvents = new EvadeStartedEvents();

        [TabGroup(Tabs.Events)]
        public EvadeStoppedEvent evadeStoppedEvent = new EvadeStoppedEvent();

		void Start () 
		{
			animator = GetComponent<Animator>();
			playerLocomotionAnimationHook = GetComponent<PlayerLocomotionAnimationHook>();
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

            //animator.SetFloat(EvadeHorizontal, playerLocomotionAnimationHook.horizontalAnimatorFloat);
            //animator.SetFloat(EvadeVertical, playerLocomotionAnimationHook.verticalAnimatorFloat);

            SnapToDirection();

			animator.Play(EvadeAnimation);

            isEvading = true;
            playerStateComponent.SetStateHard(PlayerState.Evading);
            
            if(evadeStartedEvents != null)
            {
                evadeStartedEvents.Invoke();
            }
		}

        private void SnapToDirection()
        {
            Vector3 jumpDirection = InputDriver.LocomotionOrientationDirection;

            if (InputDriver.LocomotionOrientationDirection != Vector3.zero)
            {
                Quaternion _rotation = Quaternion.LookRotation(jumpDirection);
                transform.rotation = _rotation;
            }
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

            if(evadeStoppedEvent != null)
            {
                evadeStoppedEvent.Invoke();
            }
		}
	}
}
