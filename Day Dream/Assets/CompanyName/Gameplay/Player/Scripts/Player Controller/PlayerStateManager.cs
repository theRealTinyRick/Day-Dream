using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerStateManager : MonoBehaviour 
	{
		private const string grounded = "isGrounded";
		private const string FallStrength = "FallStrength";
		
		[SerializeField]
		private float fallStrength;

		[SerializeField]
		private PlayerState currentState = PlayerState.FreeMove;
		public PlayerState CurrentState { get { return currentState; } }

		[SerializeField]
		private bool isGrounded = true;
		public bool IsGrounded { get { return isGrounded; } }

		private Animator animator;
		private Rigidbody rigidbody;
		private LayerMask playerLayer = ~ 1 << 8;

		private float previousYPosition;
		private float yDifference;

		[SerializeField]
		[Range(0,5)]
		public float checkDistance;

		[SerializeField]
		private float hardLandThreshold;
		
		[SerializeField]
		private float damagingLandThreshold;

		private void Awake () 
		{
			animator = GetComponent <Animator> ();
			rigidbody = GetComponent<Rigidbody>();

			previousYPosition = transform.position.y;
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

			// if(!isGrounded && currentState == PlayerState.FreeMove)
			// {
			// 	animator.applyRootMotion = false;
			// }
			// else
			// {
			// 	animator.applyRootMotion = true;
			// }
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
			if(Physics.Raycast(origin, Vector3.down, out hit, checkDistance)) 
			{
				if(ApplyLandAnimation())
				{
					//some logic to play different animations
					fallStrength = 2;
				}
				else
				{
					fallStrength = 0;
				}

				animator.SetBool(grounded, true);
				animator.SetFloat(FallStrength, fallStrength);

				isGrounded = true;

				previousYPosition = transform.position.y;
			}
			else
			{
				animator.SetBool(grounded, false);
				isGrounded = false;
			}
		}

		private bool ApplyLandAnimation()
		{
			float diff = previousYPosition - transform.position.y;
			if(diff >= damagingLandThreshold)
			{
				yDifference = diff;
				return true;
			}
			else if(diff > hardLandThreshold)
			{
				yDifference = diff;
				return true;
			}
			return false;
		}
		
		public void AnimOut()
		{
			ResetState();
		}
	}
}
