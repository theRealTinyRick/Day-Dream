﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraversal : MonoBehaviour {

	private PlayerManager pManager;
	private PlayerController pController;
	private PlayerMovement pMove;

	private Animator anim;
	private Rigidbody rb;

	//shimy pipe
    private Vector3 mySide;
    private Vector3 farSide;

    private float timeOfLastClimb;

	[SerializeField] ParticleSystem warpFX;

	void Start(){
		pManager = PlayerManager.instance;
		pController = GetComponent<PlayerController>();
		pMove = GetComponent<PlayerMovement>();

		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
	public IEnumerator ShimyPipeStart(GameObject pipe){
        rb.isKinematic = true;

        ShimyPipe pipeInfo = pipe.GetComponent<ShimyPipe>();
        if(Vector3.Distance(transform.position, pipeInfo.sideA.position) < Vector3.Distance(transform.position, pipeInfo.sideB.position)){
            mySide = pipeInfo.sideA.position;
            farSide = pipeInfo.sideB.position;
        }else{
            mySide = pipeInfo.sideB.position;
            farSide = pipeInfo.sideA.position;
        }

        while(Vector3.Distance(transform.position, mySide)>.1f){
            transform.position = Vector3.Lerp(transform.position, mySide, .1f);
            Quaternion rot = Quaternion.LookRotation(farSide - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, .5f);
            yield return new WaitForEndOfFrame();
        }
        PlayerManager.currentState = PlayerManager.PlayerState.Traversing;
        yield return null;
    }

    public void ShimyPipe(Vector3 movement, ShimyPipe pipe){
        if(movement.z > 0){
            transform.position = Vector3.MoveTowards(transform.position, farSide, 5 * Time.deltaTime);
            Quaternion rot = Quaternion.LookRotation(farSide - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, .5f);
        }else if(movement.z < 0){
            transform.position = Vector3.MoveTowards(transform.position, mySide, 5 * Time.deltaTime);
            Quaternion rot = Quaternion.LookRotation(mySide - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, .5f);
        }
    }

	public IEnumerator GrabLedge(GameObject ledge){
        if(!pController.CheckGrounded()){
            timeOfLastClimb = Time.time;
            RaycastHit hit;
            Vector3 origin = transform.position;
            if(Physics.Raycast(origin, transform.forward, out hit, .75F)){
                PlayerManager.currentState = PlayerManager.PlayerState.Traversing;
                rb.isKinematic = true;
                Vector3 tp = (Vector3.Distance(transform.position, hit.point) - 0.2f ) * Vector3.Normalize(hit.point - transform.position) + transform.position;
                tp.y = ledge.transform.position.y - 2.1f;
                transform.position = tp;

                pController.ledge = ledge;
                anim.Play("Grab Ledge");
            }
        }
        yield return null;
    }

    public IEnumerator MoveToNextLedge(){
        if(Time.time - timeOfLastClimb > 0.5f){
            timeOfLastClimb = Time.time;
            Vector3 tp = transform.position;
            tp.y = transform.position.y + 4;
            GetComponent<CapsuleCollider>().enabled = false;
            while(transform.position !=  tp){
                transform.position = Vector3.MoveTowards(transform.position, tp, 20 * Time.deltaTime);
                
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.forward, out hit, 0.75f)){
                    if(hit.transform.tag != "Player"){
                        //we good
                    }else
                        Drop();
                }else{
                    Drop();
                }

                yield return new WaitForEndOfFrame();
            }

            GetComponent<CapsuleCollider>().enabled = true;
            Drop();

            if(!pController.ledge){
                Drop();
            }
        }
        
        yield return null;
    }

	public void ShimyLedge(Vector3 move, Transform ledge){
        transform.rotation = ledge.rotation;
        if(move.x > 0){
            transform.Translate(Vector3.right * 3 * Time.deltaTime);
        }else if(move.x < 0){
            transform.Translate(-Vector3.right * 3 * Time.deltaTime);
        }
    }

	public IEnumerator Warp(GameObject warpPad){
        WarpPad pad = warpPad.GetComponent<WarpPad>();
        SkinnedMeshRenderer renderers = GetComponentInChildren<SkinnedMeshRenderer>();
		MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        PlayerManager.currentState = PlayerManager.PlayerState.Traversing;
        rb.isKinematic = true;
        transform.LookAt(pad.pointB.position);
        transform.position = warpPad.transform.position;
        renderers.enabled = false;
		renderer.enabled = false;
        warpFX.Play();
        yield return new WaitForSeconds(1f);
		while(Vector3.Distance(transform.position, pad.pointB.position) > .1f){
			transform.position = Vector3.Lerp(transform.position, pad.pointB.position, .05f);
			yield return new WaitForEndOfFrame();
		}
		PlayerManager.currentState = PlayerManager.PlayerState.FreeMovement;
		renderers.enabled = true;
		renderer.enabled = true;
        rb.isKinematic = false;
        warpFX.Play();
		yield return null;
    }

	public void Drop(){
        //stop climbing
        rb.isKinematic = false;
        PlayerManager.currentState = PlayerManager.PlayerState.FreeMovement;
        pController.ledge = null;
    }
}
