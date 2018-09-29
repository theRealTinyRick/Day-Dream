using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.Gameplay
{
	public class PlayerStateManager : MonoBehaviour 
	{
		private const string grounded = "isGrounded";

		[SerializeField]
		private PlayerState currentState = PlayerState.FreeMove;
		public PlayerState CurrentState { get { return currentState; } }

		[SerializeField]
		private bool isGrounded = true;
		public bool IsGrounded { get { return isGrounded; } }

		private Animator animator;
		private Rigidbody rigidbody;
		private LayerMask playerLayer = ~ 1 << 8;

		private void Awake () 
		{
			animator = GetComponent <Animator> ();
			rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate () 
		{
			CheckGrounded();

			ManagerPlayerPhyisics();
		}

		private void ManagerPlayerPhyisics()
		{
			if(currentState == PlayerState.Traversing)
			{
				rigidbody.isKinematic = true;
			}
			else
			{
				rigidbody.isKinematic = false;
			}
		}

		public void SetStateHard( PlayerState state )
		{
			currentState = state;
		}

		public void ResetState ()
		{
			currentState = PlayerState.FreeMove;
		}

		private void CheckGrounded () 
		{
			Vector3 origin = transform.position;
			origin.y += 0.5f;

			RaycastHit hit;
			if(Physics.Raycast(origin, Vector3.down, out hit, 1f)) 
			{
				animator.SetBool(grounded, true);
				isGrounded = true;
			}
			else
			{
				animator.SetBool(grounded, false);
				isGrounded = false;
			}
		}
		
		public void AnimOut()
		{
			ResetState();
		}
	}
}
