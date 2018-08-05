using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour {

	Rigidbody rb;
	Animator anim;
	PlayerTraversal pTraverse;
	PlayerController pController;
	PlayerManager pManager;
	LayerMask layerMask = 1<<8;

	void Start(){
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		pTraverse = GetComponent<PlayerTraversal>();
		pController = GetComponent<PlayerController>();
		pManager = PlayerManager.instance;
		layerMask = ~layerMask;
	}

	public bool CheckWallJump(float jumpHeight){
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 1f, layerMask)){
			if(hit.normal.y < 0.1f){
				Vector3 dir = hit.normal;
				Quaternion rot = Quaternion.LookRotation(dir);
				transform.rotation = rot;
				rb.velocity = new Vector3(dir.x * jumpHeight * 1.5f, jumpHeight, dir.z * jumpHeight * 1.5f);
				anim.Play("WallJump");
				pTraverse.Drop();
				StartCoroutine(CheckWallJump());
				return true;
			}
		}
		return false;
	}

    IEnumerator CheckWallJump(){
        while(!pController.CheckGrounded()){
            pManager.currentState = PlayerManager.PlayerState.Traversing;
			//fire a raycast to make sure player does not jump off a ledge and die
            yield return new WaitForEndOfFrame();
        }
        pManager.currentState = PlayerManager.PlayerState.FreeMovement;
        yield return null;
    }
}
