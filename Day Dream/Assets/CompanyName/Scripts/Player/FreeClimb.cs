using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimb : MonoBehaviour {

	PlayerController pController;
	FreeClimbAnimationHook animHook;
	Animator anim;
	Rigidbody rb;

	public bool isClimbing = false;

	public Transform helper;

	public Vector3 startPos;
	public Vector3 targetPos;

	public float positionOffSet;
	public float offsetFromWall = 0.3f;
	public bool isLerping = false;
	public bool inPosition;
	float climbSpeed = 3f;
	
	public float delta;
	float t = 0.0f;

	float h;
	float v;

	bool hasPlayedAnim = false;

	void Start()
	{
		helper = new GameObject().transform;
		helper.name = "Climb Helper";

		pController = GetComponent<PlayerController>();
		animHook = GetComponent<FreeClimbAnimationHook>();
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	public bool CheckForClimb()
	{
        RaycastHit hit;
        Vector3 origin = transform.position;
		if(pController.CheckGrounded())
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
		
		anim.Play("AirWallMount");
		anim.SetBool("WallClimbing", true);
	}

	void Update()
	{
		if(isClimbing)
		{
			delta = Time.deltaTime;
			Tick(delta);
		}
	}

	public void Tick(float delta)
	{
		if(!inPosition)
		{
			GetInPosition();
			return;
		}

		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");
	
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
			
		}
		else
		{
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
				Vector3 ledgePostion = transform.position;
				ledgePostion = ledgePostion + transform.forward * 0.5f;
				ledgePostion.y = o.y;

				StartCoroutine(JumpOnLedge(ledgePostion));

				return false;
			}
		}
		
		//check to see if player will hit the floor and dismount
		if(moveDir.y < 0)
		{
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

	public void Drop()
	{
		rb.isKinematic = false;
		PlayerManager.currentState = PlayerManager.PlayerState.FreeMovement;
		isClimbing = false;
		inPosition = false;
		isLerping = false;
		anim.SetBool("WallClimbing", false);
	}
	
	IEnumerator JumpOnLedge(Vector3 tp)
	{
		Drop();
		GetComponent<PlayerMovement>().Jump(40, transform.forward);
		yield return null;
	}
}
