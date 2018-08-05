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
			if(Physics.Raycast(origin, transform.position, out hit, 1, layerMask)){
				InitForClimb(hit.point, hit.normal);
			}
			return true;
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
			Vector3 dir = new Vector3(h, 0, 0);

			if(!isLerping){
				bool canMove = CanShimy(dir);
				if(!canMove || dir == Vector3.zero){
					return;
				}

				Vector3 tp = FindPosition(dir);
				shimyHelper.position = tp;
				isLerping = true;
				t = 0;
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
			}
		}
	}

	bool CanShimy(Vector3 dir){	
		RaycastHit hit; 
		if(Physics.Raycast(transform.position, dir, out hit, 2, layerMask)){
			return false;
		}
		return true;
	}

	Vector3 FindPosition(Vector3 dir){
		Vector3 result = transform.position;
		result += dir * 1.5f;

		return result;
	}

	void HandleAnim(float f){
		if(f < 0){
			anim.Play("Ledge_Left");
		}else if(f > 0){
			anim.Play("Ledge_Right");
		}
	}

	void Drop(){
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
