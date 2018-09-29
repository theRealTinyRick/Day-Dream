using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerLocomotion : MonoBehaviour 
	{
		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float moveSpeed;		

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float jumpHeight;

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float turnSpeed;

		private const string _velocityX = "velocityX";
		private const string _velocityY = "velocityY";
		private const string jump = "Jump";

		private float fallMultiplyer = 10f;
		private float lowJumpMultiplyer = 3f;
 
		private Vector3 moveDirection = new Vector3();
		private Rigidbody _rigidbody;
		private Animator _animator;
		private PlayerStateManager playerStateManager;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_animator = GetComponent<Animator>();
			playerStateManager = GetComponent<PlayerStateManager>();
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

			moveDirection  = move;	

			if(playerStateManager.CurrentState != PlayerState.FreeMove) return;

			var _velocity = new Vector3(move.x * moveSpeed, _rigidbody.velocity.y, move.z  * moveSpeed);
			_rigidbody.velocity = _velocity;

			if(move != Vector3.zero)
			{
				var _rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), turnSpeed);
				transform.rotation = _rotation;
			}
		}

		private void HandleLocomotionAnimation()
		{
			var toVector = transform.position + moveDirection;
			toVector -= transform.position;
			var crossProduct = Vector3.Cross(transform.forward, toVector);

			_animator.SetFloat(_velocityX, crossProduct.y);
			_animator.SetFloat(_velocityY, moveDirection.magnitude);
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

