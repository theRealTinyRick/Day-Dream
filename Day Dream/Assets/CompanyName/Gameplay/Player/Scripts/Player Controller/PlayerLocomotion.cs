using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerLocomotion : MonoBehaviour 
	{
		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float moveSpeed;	

		[TabGroup(Tabs.Locomotion)]
		[SerializeField]
		private float sprintSpeed;

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float jumpHeight;

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float turnSpeed;

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float minPivotSpeedAngle;

		private const string _velocityX = "velocityX";
		private const string _velocityY = "velocityY";
		private const string jump = "Jump";

		private const string pivotRun = "180 Pivot Run";
		private const string pivotJog = "180 Pivot Jog";

		private float fallMultiplyer = 10f;
		private float lowJumpMultiplyer = 3f;
 
		private Vector3 moveDirection = new Vector3();
		public Vector3 MoveDirection { get { return moveDirection; } }

		private Rigidbody _rigidbody;
		private Animator _animator;
		private PlayerStateManager playerStateManager;
		private PlayerController playerController;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_animator = GetComponent<Animator>();
			playerStateManager = GetComponent<PlayerStateManager>();
			playerController = GetComponent<PlayerController>();
		}

		private void Update () 
		{
			HandleLocomotionAnimation();
		}

		private void FixedUpdate () 
		{
			BetterJumpPhysics();
		}

		public void PlayerMove(Transform playerCamera, Vector3 move)
		{
			Transform tempCam = playerCamera;
			tempCam.position  = transform.position;
			tempCam.eulerAngles = new Vector3(0, tempCam.rotation.eulerAngles.y,0);

			move = tempCam.TransformDirection(move);
			move.y = 0.0f;

			moveDirection = move;	

			if(playerStateManager.CurrentState != PlayerState.FreeMove) return;

			float _speed = playerController.IsSprinting ? sprintSpeed : moveSpeed;

			var _velocity = new Vector3(move.x * _speed, _rigidbody.velocity.y, move.z  * _speed);
			_rigidbody.velocity = _velocity;

			if(move != Vector3.zero)
			{
				var _rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), turnSpeed);
				transform.rotation = _rotation;
			}

			WarpPivotAnimation();
		}

		private void HandleLocomotionAnimation()
		{
			var toVector = transform.position + moveDirection;
			toVector -= transform.position;
			var crossProduct = Vector3.Cross(transform.forward, toVector);

			if(!_animator.GetCurrentAnimatorStateInfo(0).IsName(pivotRun) && !_animator.GetCurrentAnimatorStateInfo(0).IsName(pivotJog) && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump Loop") && playerController.IsGrounded)
			{
				float angle = Vector3.Angle(transform.forward, toVector);

				if(angle > minPivotSpeedAngle)
				{
					if(playerController.IsSprinting)
					{
						_animator.Play(pivotRun);
					}
					else
					{
						_animator.Play(pivotJog);
					}
				}
			}

			float x = crossProduct.y;
			float y = moveDirection.magnitude;

			//adjust speed for sprinting
			if(!playerController.IsSprinting)
			{
				if(y > 0.66f)
				{
					y = 0.66f;
				}
			}

			_animator.SetFloat(_velocityX, x);
			_animator.SetFloat(_velocityY, y);
		}

		private void WarpPivotAnimation()
		{
			// if(_animator.GetCurrentAnimatorStateInfo(0).IsName(pivotRun))
			// {
			// 	float startTime = 25/100;
			// 	float endTime = 70/100;

			// 	var _rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection, Vector3.up), turnSpeed);
				
			// 	_animator.MatchTarget( transform.position, _rotation, AvatarTarget.Body, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			// }
		}

		public void Jump()
		{	
			if( playerStateManager.CurrentState != PlayerState.FreeMove ) return;
			if( !playerStateManager.IsGrounded ) return;

			var _velocity = new Vector3(_rigidbody.velocity.x, jumpHeight, _rigidbody.velocity.z);
			_rigidbody.velocity = _velocity;

			_animator.Play(jump);
		}

		private void BetterJumpPhysics ()
		{
			if (_rigidbody.velocity.y  < 0) 
            	_rigidbody.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
			else if (_rigidbody.velocity.y  > 0 )
				_rigidbody.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
		}
	}
}

