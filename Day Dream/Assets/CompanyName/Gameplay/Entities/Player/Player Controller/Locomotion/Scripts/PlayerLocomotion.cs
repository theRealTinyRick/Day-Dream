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
        private float airSpeed;

		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float turnDamping;

        [TabGroup(Tabs.Locomotion)]
        [SerializeField]
        private bool isSprinting;

        /// <summary>
        /// The list of states that the player can move in
        /// </summary>
        [Tooltip("The list of states that the player can move in")]
        [SerializeField]
        private PlayerState[] availableStates;

		public Vector3 playerOrientationDirection = new Vector3();
		public Vector3 playerOrientationDirectionNotNormalized = new Vector3();

        [TabGroup(Tabs.Locomotion)]
        [SerializeField]
        private Transform LocomotionOrientationController;

		private Animator animator;
		private Rigidbody _rigidbody;

		private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;
		private PlayerAttackAnimationController playerAttackAnimationController;
        private PlayerGroundedComponent playerGroundedComponent;
		private PlayerEvade playerEvade;
        private PlayerStateComponent playerStateComponent;
        private PlayerJump playerJump;

		private void Start () 
		{
			animator = GetComponentInChildren<Animator>();
			_rigidbody = GetComponentInChildren<Rigidbody>();

			playerLocomotionAnimationHook = GetComponentInChildren<PlayerLocomotionAnimationHook>();
			playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
			playerEvade = GetComponent<PlayerEvade>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
            playerJump = GetComponent<PlayerJump>();

            if(LocomotionOrientationController == null)
            {
                LocomotionOrientationController = new GameObject().transform;
                LocomotionOrientationController.name = "Locomotion Orientation Controller";
            }
		}
		
		private void FixedUpdate () 
		{
			Vector3 _direction = GetOrientationDirection();

            if (CanMove())
            {
			    Move(_direction);
			
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

        private void Move(Vector3 direction)
		{
            float _speed = baseSpeed;

            if(playerJump.playerJumped && !playerJump.shouldUseLocomotionControlInTheAir)
            {
                 return;
            }

            if(!playerGroundedComponent.IsGrounded)
            {
                _speed = airSpeed;
            }
            else if(isSprinting)
            {
                _speed = sprintSpeed;
            }

            if(InputDriver.LocomotionDirection != Vector3.zero)
            {

		        _rigidbody.velocity =
			        new Vector3( (direction.x * _speed) * InputDriver.LocomotionDirection.magnitude, 
			        _rigidbody.velocity.y, 
			        (direction.z * _speed) * InputDriver.LocomotionDirection.magnitude );
            }
            else
            {
               // _rigidbody.velocity = Vector3.zero;
            }
		}

        // we do this  to move the player faster forward as long as they were moving forward in the air
        private float FindAirSpeed()
        {
            Vector3 _forward = transform.forward;
            Vector3 _moveDirection = InputDriver.LocomotionOrientationDirection;

            if(_moveDirection != Vector3.zero)
            {
                float _dot = Vector3.Dot(_forward, _moveDirection);

                if(_dot <= 0.1f)
                {
                    _dot = 0.1f;
                }

                return airSpeed * _dot;
            }

            return 0.0f;
        }

		private void RotatePlayer()
		{
            if(playerGroundedComponent.IsGrounded)
            {
			    transform.rotation = Quaternion.Lerp(transform.rotation, GetOrientationRotation(), turnDamping);
            }
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
            SetLocomotionOrientationControllerRotation();

			Vector3 _direction = InputDriver.LocomotionDirection;
			_direction = LocomotionOrientationController.TransformDirection(_direction).normalized;
			_direction.y = 0;

			InputDriver.LocomotionOrientationDirection = _direction;
			playerOrientationDirection = _direction;

			return _direction;
		}

        private void SetLocomotionOrientationControllerRotation()
        {
            Quaternion _targetRotation = EntityManager.Instance.GameCamera.transform.rotation;
            _targetRotation.x = 0;
            _targetRotation.z = 0;
            LocomotionOrientationController.rotation = _targetRotation;
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
            //if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
            //{
            //    return false;
           // }

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
