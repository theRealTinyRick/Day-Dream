using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimb : MonoBehaviour {

	private bool isClimbing = false;
	public bool IsClimbing{
		get{return isClimbing;}
	}

	bool isLerping = false;

	PlayerController pController;
	PlayerManager pManager;
	PlayerMovement pMove;
	WallJump wallJump;
	Animator anim;
	Rigidbody rb;
	Ledge ledge;
	LayerMask layerMask = 1<<8;
	Transform shimyHelper;

	float t;
	float speed = 1.5f;
	bool hasPlayedAnim = false;

	void Start(){
		layerMask = ~layerMask;

		pController = GetComponent<PlayerController>();
		pMove = GetComponent<PlayerMovement>();
		wallJump = GetComponent<WallJump>();
		pManager = PlayerManager.instance;
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();

		shimyHelper = new GameObject().transform;
		shimyHelper.name = "ShimyHelper";
	}

	public bool CheckForClimb(){
		if(!pController.CheckGrounded()){
			RaycastHit hit;
			Vector3 origin = transform.position;
			origin.y += 1;
			if(Physics.Raycast(origin, transform.forward, out hit, 1, layerMask)){
				InitForClimb(hit.point, hit.normal);
				return true;
			}
		}
		return false;
	}

	void InitForClimb(Vector3 tp, Vector3 normal){
		rb.isKinematic = true;
		pManager.currentState = PlayerManager.PlayerState.Traversing;
		anim.Play("GrabLedge");
		anim.SetBool("LedgeClimbing", true);
		tp.y = ledge.transform.position.y - 2.1f;
		tp -= transform.forward * 0.2f;
		transform.position = tp;
		shimyHelper.position = tp;

		Quaternion rot = Quaternion.LookRotation(-normal);
		transform.rotation = rot;

		isClimbing = true;
	}
	
	void Update () {
		Tick();
	}

	void Tick(){
		if(isClimbing){
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			Vector3 dir = new Vector3(h, 0, 0);

			if(v > 0 && Input.GetButtonDown("Jump")){
				pMove.Jump(pController.jumpHieght); 
			}else if(v < 0){
				Drop();
			}else if(v == 0 && Input.GetButtonDown("Jump")){
				wallJump.CheckWallJump(pController.jumpHieght/1.5f);
			}

			if(!isLerping){
				bool canMove = CanShimy(dir);
				if(!canMove || dir == Vector3.zero){
					return;
				}

				t = 0;
				isLerping = true;
				hasPlayedAnim = false;

			}else{
				t += Time.deltaTime;
				if(t > 1){
					// t = 1;
					isLerping = false;
				}

				if(!hasPlayedAnim){
					HandleAnim(h);
					hasPlayedAnim = true;
				}
				transform.position = Vector3.Lerp(transform.position, shimyHelper.position, t);
				transform.rotation = shimyHelper.rotation;
			}
		}
	}

	bool CanShimy(Vector3 dir){	
		RaycastHit hit; 
		Vector3 origin = transform.position;

		if(dir.x < 0){
			dir = -transform.right;
		}else if(dir.x > 0){
			dir = transform.right;
		}else
			return false;

		if(Physics.Raycast(transform.position, dir, out hit, 1.5f, layerMask)){
			return false;
		}

		origin += dir * 1;

		if(Physics.Raycast(origin, transform.forward, out hit, 1, layerMask)){
			Quaternion rot = Quaternion.LookRotation(-hit.normal);
			shimyHelper.position = PositionWithOffset(origin, hit.point);
			shimyHelper.rotation = rot;

			return true;
		}

		return false;
	}

	Vector3 PositionWithOffset(Vector3 origin, Vector3 target){
		Vector3 direction = origin - target;
		direction.Normalize();
		Vector3 offset = direction * 0.2f;

		return target + offset;
	}

	void HandleAnim(float f){
		if(f < 0){
			anim.Play("Ledge_Left");
		}else if(f > 0){
			anim.Play("Ledge_Right");
		}
	}

	public void Drop(){
		isClimbing = false;
		rb.isKinematic = false;
		anim.SetBool("LedgeClimbing", false);
		pManager.currentState = PlayerManager.PlayerState.FreeMovement;
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "Ledge"){
			ledge = other.transform.GetComponent<Ledge>();
			CheckForClimb();
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "Ledge")
			Drop();
	}
}
