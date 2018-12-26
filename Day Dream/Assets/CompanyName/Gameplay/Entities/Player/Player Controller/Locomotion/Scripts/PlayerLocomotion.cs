using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

namespace AH.Max.Gameplay
{
	
	public class PlayerLocomotion : MonoBehaviour 
	{
		[TabGroup("TestIng")]
		[SerializeField]
		private bool LockedOn; // Remove

		[TabGroup("TestIng")]
		[SerializeField]
		private bool defending; // Remove

		[TabGroup("TestIng")]
		[SerializeField]
		private bool attacking;	// Remove
		
		[TabGroup("TestIng")]
		[SerializeField]
		private Transform target; // Remove

		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float baseSpeed;

		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float sprintSpeed;

		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float turnDamping;

        /// <summary>
        /// The list of states that the player can move in
        /// </summary>
        [Tooltip("The list of states that the player can move in")]
        [SerializeField]
        private PlayerState[] availableStates;

		public Vector3 playerOrientationDirection = new Vector3();
		public Vector3 playerOrientationDirectionNotNormalized = new Vector3();

		private Animator animator;
		private Rigidbody _rigidbody;

		private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;
		private PlayerAttackAnimationController playerAttackAnimationController;
		private PlayerEvade playerEvade;
        private PlayerStateComponent playerStateComponent;


		private void Start () 
		{
			animator = GetComponentInChildren<Animator>();
			_rigidbody = GetComponentInChildren<Rigidbody>();

			playerLocomotionAnimationHook = GetComponentInChildren<PlayerLocomotionAnimationHook>();
			playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
			playerEvade = GetComponent<PlayerEvade>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
		}
		
		private void FixedUpdate () 
		{
			Vector3 _direction = GetOrientationDirection();

            if (CanMove())
            {
			    Move(LockedOn, _direction);
			
			    if(LockedOn)
			    {
				    if(defending || attacking)
				    {
					    FaceTarget();
					    return;
				    }
			    }
			
			    RotatePlayer();
            }
		}

        private void Move(bool lockedOn, Vector3 direction)
		{
		    _rigidbody.velocity =
			    new Vector3( (direction.x * baseSpeed) * InputDriver.LocomotionDirection.magnitude, 
			    _rigidbody.velocity.y, 
			    (direction.z * baseSpeed) * InputDriver.LocomotionDirection.magnitude ) ;
		}

		private void RotatePlayer()
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, GetOrientationRotation(), turnDamping);
		}

		private void FaceTarget()
		{
			Vector3 _direction = target.position - transform.position;
			_direction.y = 0;
			Quaternion _rotation = Quaternion.LookRotation(_direction);

			transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, turnDamping);
		}
		
		private Vector3 GetOrientationDirection()
		{
			Vector3 _direction = InputDriver.LocomotionDirection;
			_direction = EntityManager.Instance.GameCamera.transform.TransformDirection(_direction).normalized;
			_direction.y = 0;

			InputDriver.LocomotionOrientationDirection = _direction;
			playerOrientationDirection = _direction;

			return _direction;
		}

		private Quaternion GetOrientationRotation()
		{
			return Quaternion.LookRotation(GetOrientationDirection());
		}

		///<Summary>
		/// Is the player currently in a state where movement should be applied
		///</Summary>
		private bool CanMove()
		{
            if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
            {
                return false;
            }

            foreach (var _state in availableStates)
            {
                if(playerStateComponent.CheckState(_state))
                {
			        if(playerOrientationDirection == Vector3.zero)
			        {
				        return false;
			        }

                    return true;
                }
            }
			
			return false;
		}
	}
}
