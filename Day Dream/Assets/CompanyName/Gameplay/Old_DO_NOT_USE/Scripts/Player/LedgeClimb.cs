using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimb : MonoBehaviour {

	[SerializeField]
	private float yOffset;

	[SerializeField]
	private float wallOffset;

	[SerializeField]
	private bool isClimbing = false;
	public bool IsClimbing{ get{return isClimbing;} }

	private bool isLerping = false;

	private PlayerController pController;
	private PlayerManager pManager;
	private PlayerMovement pMove;
	private WallJump wallJump;
	private Animator anim;
	private Rigidbody rb;
	private Ledge ledge;
	private LayerMask layerMask = 1 << 8;
	private Transform shimyHelper;

	private float t;
	private float speed = 1.5f;
	private bool hasPlayedAnim = false;

	private void Start(){
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
				shimyHelper.position = hit.point;
				InitForClimb(hit);
				return true;
			}
		}
		return false;
	}

	private void InitForClimb(RaycastHit hit){
		rb.isKinematic = true;
		PlayerManager.currentState = PlayerManager.PlayerState.Traversing;

		anim.Play("GrabLedge");
		anim.SetBool("LedgeClimbing", true);

		Vector3 tp = hit.point;
		tp += hit.normal * wallOffset;
		tp.y = ledge.leftSide.position.y;
		tp.y -= yOffset;

		Quaternion rot = ledge.leftSide.rotation;

		transform.position = tp;
		transform.rotation = rot;

		isClimbing = true;
	}
	
	private void Update () {
		Tick();
	}

	private void Tick(){
		if(isClimbing){
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			Vector3 dir = new Vector3(h, 0, 0);

			if(v < 0){
				Drop();
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
				transform.rotation = ledge.leftSide.rotation;
			}
		}
	}

	private bool CanShimy(Vector3 dir){	
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

	private Vector3 PositionWithOffset(Vector3 origin, Vector3 target){
		Vector3 direction = origin - target;
		direction.Normalize();
		Vector3 offset = direction * 0.2f;

		return target + offset;
	}

	private void HandleAnim(float f){
		if(f < 0){
			anim.Play("Ledge_Left");
		}else if(f > 0){
			anim.Play("Ledge_Right");
		}
	}

	public IEnumerator ClimbUpLedge(){
		Vector3 dir = transform.forward;
		Vector3 o = transform.position;
		o.y += 3;

		RaycastHit hit;
		if(Physics.Raycast(o, dir, out hit, 1, layerMask)){

			pMove.Jump(pController.jumpHieght); 
			Drop();
			
			yield break;
		}
		
		o += transform.forward;
		dir = Vector3.down;
		anim.applyRootMotion = true;
		CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();

		foreach(CapsuleCollider collider in colliders){
			collider.enabled = false;
		}

		anim.Play("ClimbOnLedge");

		yield return new WaitForSeconds(2);
		// transform.position = o;
		
		foreach(CapsuleCollider collider in colliders){
			collider.enabled = true;
		}
		Drop();

		yield return null;
	}

	public void Drop(){
		isClimbing = false;
		rb.isKinematic = false;
		anim.SetBool("LedgeClimbing", false);
		anim.applyRootMotion = false;
		PlayerManager.currentState = PlayerManager.PlayerState.FreeMovement;
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
