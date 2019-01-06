using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    public class PlayerLocomotionAnimationHook : MonoBehaviour
    {
        public const string LockedOn = "LockedOn";
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";

        [TabGroup("Animation")]
        [SerializeField]
        public float verticalAnimatorFloat = 0;

        [TabGroup("Animation")]
        [SerializeField]
        public float horizontalAnimatorFloat = 0;

        [TabGroup("Animation")]
        [SerializeField]
        [Range(0, 1)]
        private int lockedOnAnimatorFloat = 0; // for testing purposes

		[TabGroup("Animation")]
		[SerializeField]
		private bool ShouldLean; // determines if the player should lean while running.

        private PlayerLocomotion playerLocomotion;
        private PlayerStateComponent playerStateComponent;
        private PlayerAttackAnimationController playerAttackAnimatorController;
        private PlayerEvade playerEvade;

        private Animator animator;

        [SerializeField]
        private PlayerState[] states;

		private void Start()
		{
			playerLocomotion = GetComponent<PlayerLocomotion>();
			playerStateComponent= GetComponent<PlayerStateComponent>();
			playerAttackAnimatorController = GetComponent<PlayerAttackAnimationController>();
			playerEvade = GetComponent<PlayerEvade>();
			animator = GetComponent<Animator>();
		}

		private void Update () 
		{
			LocomotionAnimation();
			ApplyAnimationFloats();
		}

		private void LocomotionAnimation()
		{
			Vector3 _forwardVector = transform.forward;
			Vector3 _moveDirection = playerLocomotion.playerOrientationDirection * InputDriver.LocomotionDirection.magnitude; //multiply here so we can dampen some of the values.
			Vector3 _crossProduct = Vector3.Cross(_forwardVector, _moveDirection);

			Debug.DrawRay(transform.position, _moveDirection, Color.red);
			Debug.DrawRay(transform.position, _forwardVector, Color.blue);
			Debug.DrawRay(transform.position, _crossProduct, Color.green);

			if(lockedOnAnimatorFloat == 0)
			{
                verticalAnimatorFloat = _moveDirection.magnitude;
				horizontalAnimatorFloat = ShouldLean ? _crossProduct.y : 0;
			}
			else
			{
				verticalAnimatorFloat = _moveDirection.magnitude * Vector3.Dot(_forwardVector, _moveDirection);
				horizontalAnimatorFloat = _crossProduct.y;
			}
		}

		private void ApplyAnimationFloats()
		{
			if(CheckState())
			{
				animator.SetFloat(Horizontal, 0);
				animator.SetFloat(Vertical, 0);
				return;
			}

			animator.SetFloat(LockedOn, (float)lockedOnAnimatorFloat);
			animator.SetFloat(Horizontal, Mathf.Lerp(animator.GetFloat(Horizontal), horizontalAnimatorFloat, 0.2f));
			animator.SetFloat(Vertical, Mathf.Lerp(animator.GetFloat(Vertical), verticalAnimatorFloat, 0.2f));
		}

        private bool CheckState()
        {
            foreach(var _state in states)
            {
                if(playerStateComponent.CheckState(_state))
                {
                    return true;
                }
            }

            return false;
        }
	}
}
