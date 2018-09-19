using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.Gameplay
{
	public class PlayerFreeClimb : MonoBehaviour 
	{
		private const string ClimbLedge = "ClimbOnLedge";

		public float positionOffSet;
		public float offsetFromWall = 0.3f;
		public bool isLerping = false;
		public bool inPosition;
		
		private float climbSpeed = 3f;
		
		public float delta;
		private float t = 0.0f;

		private float h;
		private float v;

		private bool hasPlayedAnim = false;
		private bool isClimbingOnEdge = false;
		public bool isClimbing = false;

		private PlayerController pController;
		private FreeClimbAnimationHook animHook;
		private Animator anim;
		private Rigidbody rb;
		private PlayerStateManager playerStateManager;
		private Animator _animator;
		private Rigidbody _rigidbody;

		public Transform helper;
		public Vector3 startPos;
		public Vector3 targetPos;

		private void Start()
		{
			helper = new GameObject().transform;
			helper.name = "Climb Helper";

			playerStateManager = GetComponent <PlayerStateManager> ();
			animHook = GetComponent <FreeClimbAnimationHook> ();
			anim = GetComponent <Animator> ();
			rb = GetComponent <Rigidbody> ();
		}

		public bool CheckForClimb()
		{
			RaycastHit hit;
			Vector3 origin = transform.position;
			
			if(playerStateManager.IsGrounded)
			{
				origin.y += 2; 
			}

			if(Physics.Raycast(origin, transform.forward, out hit, 1))
			{
				if(hit.transform.tag == "Climbable")
				{
					InitForClimb(hit);
					return true;
				}
			}
			return false;
		}

		public void InitForClimb(RaycastHit hit)
		{
			isClimbing = true;
			rb.isKinematic = true;
			helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
			startPos = transform.position;
			targetPos = hit.point + (hit.normal * offsetFromWall);
			t = 0;
			inPosition = false;

			playerStateManager.SetStateHard(PlayerStateManager.PlayerState.Traversing);
	
			anim.Play("AirWallMount");
			anim.SetBool("WallClimbing", true);
		}

		private void Update()
		{
			if(isClimbing)
			{
				delta = Time.deltaTime;
				Tick(delta);
			}
		}

		public void Tick(float delta)
		{
			if(isClimbingOnEdge) return;

			if(!inPosition)
			{
				GetInPosition();
				return;
			}

			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
		
			if(!isLerping && anim.GetCurrentAnimatorStateInfo(0).IsName("FreeClimbIdle"))
			{
				hasPlayedAnim = false;

				Vector3 horizontal = helper.right * h;
				Vector3 vertical = helper.up * v;
				Vector3 moveDir = (horizontal + vertical).normalized;

				bool canMove = CanMove(moveDir);
				if(!canMove || moveDir == Vector3.zero)
				{
					return;
				}

				t = 0;
				isLerping = true;
				startPos = transform.position;
				targetPos = helper.position;
				
			}else{
				t += delta * climbSpeed;
				if( t > 1)
				{
					t = 1;
					isLerping = false;
				}
				
				if(!hasPlayedAnim)
				{
					animHook.HandleAnimation(h, v);
					hasPlayedAnim = true;
				}
				
				Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
				transform.position = cp;
				transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
			}
		}

		bool CanMove(Vector3 moveDir)
		{
			int layermask = 1<<8;
			layermask = ~layermask;

			if(moveDir.y > 0)
			{
				Vector3 o = transform.position;
				o.y += 2.5f;
				RaycastHit ledgeHit;
				Debug.DrawRay(o, transform.forward, Color.green, 5);
				if(Physics.Raycast(o, transform.forward * 5, out ledgeHit, 5, layermask))
				{
					if(ledgeHit.transform.tag != "Climbable")
					{
						return false;
					}
				}
				else
				{
					StartCoroutine(JumpOnLedge());
					return false;
				}
			}
			
			if(moveDir.y < 0)
			{
				//check to see if player will hit the floor and dismount
				Vector3 origin1 = transform.position;
				RaycastHit hitFloor;		
				if(Physics.Raycast(origin1, -Vector3.up, out hitFloor, positionOffSet, layermask))
				{
					Drop();
					return false;
				}
			}

			//check dir of input and determine if the player will hit an obstical
			Vector3 origin = transform.position;
			float dis = positionOffSet;
			Vector3 dir = moveDir;
			Debug.DrawRay(origin, dir * dis, Color.red, 5);
			RaycastHit hit;

			if(Physics.Raycast(origin, dir, out hit, dis, layermask))
			{

				if(hit.transform.tag != "Climbable")
				{
					return false;
				}

				helper.position = PosWithOffset(origin, hit.point);
				helper.rotation = Quaternion.LookRotation(-hit.normal);
				return true;
			}

			origin += moveDir * dis;
			dir = helper.forward;
			float dis2 = 1;

			Debug.DrawRay(origin, dir * dis2, Color.blue, 5);
			if(Physics.Raycast(origin, dir, out hit, dis2))
			{
				
				if(hit.transform.tag != "Climbable")
				{
					return false;
				}

				helper.position = PosWithOffset(origin, hit.point);
				helper.rotation = Quaternion.LookRotation(-hit.normal);
				return true;
			}

			return false;
		} 

		void GetInPosition()
		{
			t += delta;
			if(t > 1){
				t = 1;
				inPosition = true;
			}

			Vector3 tp = Vector3.Lerp(startPos, targetPos, t * 9);
			transform.position = tp;

			tp.y = transform.position.y;
			transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
		}

		Vector3 PosWithOffset(Vector3 origin, Vector3 target)
		{
			Vector3 direction = origin - target;
			direction.Normalize();
			Vector3 offset = direction * offsetFromWall;
			return target + offset;
		}


		
		IEnumerator JumpOnLedge()
		{
			isClimbingOnEdge = true;

			int layermask = 1<<8;
			layermask = ~layermask;

			Vector3 tp = new Vector3();

			Vector3 origin = transform.position;
			origin.y += 3f;
			origin += transform.forward;

			RaycastHit hit;

			if(Physics.Raycast(origin, Vector3.down, out hit, 5,layermask))
			{
				tp = hit.point;
			}

			origin = transform.position;
			if(Physics.Raycast(origin, transform.forward, out hit, 5, layermask))
			{
				tp.x = hit.point.x;
				tp.z = hit.point.z;

				helper.position = tp;
			}

			Vector3 endPoint = tp;
			endPoint += transform.forward * 0.2f;

			tp = LedgeWithOffset(tp);
			while(Vector3.Distance(transform.position, tp) > 0.05f)
			{
				transform.position = Vector3.Lerp(transform.position, tp, 0.2f);
				yield return new WaitForEndOfFrame();
			}

			anim.SetTrigger(ClimbLedge);

			yield return new WaitForSeconds(2f);

			while(Vector3.Distance(transform.position, endPoint) > 0.05f)
			{
				transform.position = Vector3.Lerp(transform.position, endPoint, 0.2f);
				yield return new WaitForEndOfFrame();
			}

			Drop();
			
			yield return null;
		}

		private Vector3 LedgeWithOffset (Vector3 ledge)
		{
			ledge += -transform.forward * offsetFromWall;
			ledge.y -= 2;
			return ledge;
		}

		public void Drop()
		{
			rb.isKinematic = false;
			isClimbing = false;
			inPosition = false;
			isLerping = false;
			isClimbingOnEdge = false;
			anim.SetBool("WallClimbing", false);
			playerStateManager.ResetState();
		}
	}
}