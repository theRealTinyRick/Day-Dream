using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.Gameplay
{
	public class PlayerStateManager : MonoBehaviour 
	{
		private const string grounded = "isGrounded";

		public enum PlayerState 
		{
			FreeMove, /*This is the normal state of the player where any basic action can be acrries out */
			Traversing, /*This is the state where the player is climbing or another simlar action. Only those actions can be acrries out */
			Attacking /*When in this state player is in the freemove state the player may freely move to this one while attacking
			Other actions cannot be activated until the player has left this state which is done once an attack is finished */
		}

		[SerializeField]
		private PlayerState currentState = PlayerState.FreeMove;
		public PlayerState CurrentState { get { return currentState; } }

		[SerializeField]
		private bool isGrounded = true;
		public bool IsGrounded { get { return isGrounded; } }

		private Animator _animator;
		private LayerMask playerLayer = ~ 1 << 8;

		private void Awake () 
		{
			_animator = GetComponent <Animator> ();
		}

		private void FixedUpdate () 
		{
			CheckGrounded();
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
				_animator.SetBool(grounded, true);
				isGrounded = true;
			}
			else
			{
				_animator.SetBool(grounded, false);
				isGrounded = false;
			}
		}
	}
}
