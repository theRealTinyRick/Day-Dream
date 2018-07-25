using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimb : MonoBehaviour {
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

	void Start(){
		helper = new GameObject().transform;
		helper.name = "Climb Helper";
	}

	public void InitForClimb(RaycastHit hit){
		isClimbing = true;
		GetComponent<Rigidbody>().isKinematic = true;
		helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
		startPos = transform.position;
		targetPos = hit.point + (hit.normal * offsetFromWall);
		t = 0;
		inPosition = false;
	}

	void Update(){
		if(isClimbing){
			delta = Time.deltaTime;
			Tick(delta);
		}
	}

	public void Tick(float delta){
		if(!inPosition){
			GetInPosition();
			return;
		}
		
		if(!isLerping){
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
			float m = Mathf.Abs(h) + Mathf.Abs(v);

			Vector3 horizontal = helper.right * h;
			Vector3 vertical = helper.up * v;
			Vector3 moveDir = (horizontal + vertical).normalized;

			bool canMove = CanMove(moveDir);
			if(!canMove || moveDir == Vector3.zero){
				return;
			}

			t = 0;
			isLerping = true;
			startPos = transform.position;
			Vector3 tp = helper.position - transform.position;
			float d = Vector3.Distance(helper.position, startPos) / 2;
			tp *= positionOffSet;
			tp += transform.position;
			targetPos = helper.position;
			
		}else{
			t += delta * climbSpeed;
			if( t > 1){
				t = 1;
				isLerping = false;
			}

			Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
			transform.position = cp;
			transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
		}
	}

	bool CanMove(Vector3 moveDir){
		int layermask = 1<<8;
		layermask = ~layermask;

		if(moveDir.y > 0){
			Vector3 o = transform.position;
			o.y += 2.5f;

			RaycastHit ledgeHit;
			Debug.DrawRay(o, transform.forward, Color.green, 5);
			if(Physics.Raycast(o, transform.forward * 5, out ledgeHit, 1, layermask)){
				
			}else{
				return false;
			}
		}

		Vector3 origin = transform.position;
		float dis = positionOffSet;
		Vector3 dir = moveDir;
		Debug.DrawRay(origin, dir * dis, Color.red, 5);
		RaycastHit hit;

		if(Physics.Raycast(origin, dir, out hit, dis, layermask)){
			Debug.Log(hit.transform.tag);
			if(moveDir.y < 0)
				Drop();
			return false;
		}

		origin += moveDir * dis;
		dir = helper.forward;
		float dis2 = 1;

		Debug.DrawRay(origin, dir * dis2, Color.blue, 5);
		if(Physics.Raycast(origin, dir, out hit, dis2)){
			Debug.Log(hit);
			helper.position = PosWithOffset(origin, hit.point);
			helper.rotation = Quaternion.LookRotation(-hit.normal);
			return true;
		}

		origin += dir * dis2;
		dir = -Vector3.up;

		if(moveDir.y < 0){
			Debug.DrawRay(origin, dir,Color.yellow);
			if(Physics.Raycast(origin, dir, out hit, 1)){
				float angle = Vector3.Angle(helper.up, hit.normal);
				if(angle < 40){
					helper.position = PosWithOffset(origin, hit.point);
					helper.rotation = Quaternion.LookRotation(-hit.normal);
					Drop();
					return false;
				}
			}
		}

		return false;
	} 

	void GetInPosition(){
		t += delta;
		if(t > 1){
			t = 1;
			inPosition = true;
		}

		Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
		transform.position = tp;

		tp.y = transform.position.y;
		transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
	}

	Vector3 PosWithOffset(Vector3 origin, Vector3 target){
		Vector3 direction = origin - target;
		direction.Normalize();
		Vector3 offset = direction * offsetFromWall;
		return target + offset;
	}

	public void Drop(){
		GetComponent<Rigidbody>().isKinematic = false;
		isClimbing = false;
		inPosition = false;
	}
	
	IEnumerator JumpOnLedge(){
		Debug.Log("jump on ledge");
		yield return null;
	}
}
