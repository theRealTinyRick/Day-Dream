using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    PlayerController pController;
    Rigidbody rb;
    GameObject pCamera;

    private float fallMultiplyer = 10f;
    private float lowJumpMultiplyer = 3f;

    [SerializeField] ParticleSystem warpFX;
    [SerializeField] ParticleSystem hitFX;

    //shimy pipe
    Vector3 mySide;
    Vector3 farSide;

    private void Start(){
        pController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        pCamera = Camera.main.gameObject;
    }

    private void FixedUpdate(){
        BetterJumpPhysics();
    }

    public void FreeMovement(Vector3 movement, float speed){
        if (PlayerManager.instance.currentState != PlayerManager.PlayerState.Attacking){
            if (movement.x != 0 && movement.z != 0)
                speed -= speed / 3;
            
            Vector3 dir = pCamera.transform.position - transform.position;
            dir.y = 0;
            movement = pCamera.transform.TransformDirection(movement);
            movement.y = 0;

            rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);

            if (movement != Vector3.zero){
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), .3f);
                pController.anim.SetBool("isMoving", true);
            }else{
                pController.anim.SetBool("isMoving", false);
            }
        }
        else{
            pController.anim.SetBool("isMoving", false);
        }
    }

    public void Jump(float jumpHeight){
        rb.velocity = new Vector3(0,jumpHeight, 0);
        // rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        pController.anim.SetBool("isGrounded", false);
        pController.anim.Play("Jump");
    }

    public void Evade(float evadeStength){
        // rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * (evadeStength + (evadeStength * 0.5f)), .5f);
        pController.anim.Play("Roll");
        StartCoroutine(Invulnerabe());
    }

    private IEnumerator Invulnerabe(){
        PlayerManager.instance.isVulnerable = false;
        yield return new WaitForSeconds(.5f);
        PlayerManager.instance.isVulnerable = true;
    }

    public void LookAtTarget(Transform target){
        Vector3 tp = target.position;
        tp.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(tp - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, .2f);
    }

    public IEnumerator Warp(GameObject warpPad){
        WarpPad pad = warpPad.GetComponent<WarpPad>();
        SkinnedMeshRenderer renderers = GetComponentInChildren<SkinnedMeshRenderer>();
		MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Traversing;
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
		PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
		renderers.enabled = true;
		renderer.enabled = true;
        rb.isKinematic = false;
        warpFX.Play();
		yield return null;
    }

    public IEnumerator ZipLine(){

        yield return null;
    }

    public IEnumerator LadderStart(GameObject ladder){
        Ladder ladderInfo = ladder.GetComponent<Ladder>();
        Vector3 startSide = new Vector3(0,0,0);
        if(Vector3.Distance(transform.position, ladderInfo.topPos.position) < Vector3.Distance(transform.position, ladderInfo.bottomPos.position)){
            startSide = ladderInfo.topPos.position;
        }else{
            startSide = ladderInfo.bottomPos.position;
        }
        pController.anim.SetBool("isClimbing", true);
        rb.isKinematic = true;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.CanNotMove;
        while(Vector3.Distance(transform.position, startSide)>.1f){
            transform.position = Vector3.Lerp(transform.position, startSide, .1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, ladderInfo.topPos.rotation, .5f);
            yield return new WaitForEndOfFrame();
        }
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Traversing;
        yield return null;
    }

    public void ClimbLadder(Vector3 move, GameObject ladder){
        //add ending the ladder climb
        Ladder ladderInfo = ladder.GetComponent<Ladder>();
        transform.rotation = Quaternion.Lerp(transform.rotation, ladderInfo.topPos.rotation, .5f);
        if(move.z > 0){
            transform.position = Vector3.MoveTowards(transform.position, ladderInfo.topPos.position, 2 * Time.deltaTime);
            pController.anim.SetBool("isClimbingUp", true);
            pController.anim.SetBool("isClimbingDown", false);
            pController.anim.speed = Mathf.Lerp(pController.anim.speed, 1.5f, .3f);
            if(Vector3.Distance(transform.position, ladderInfo.topPos.position) <= .1){
                LadderEnd();
                Jump(20);
            }
        }else if(move.z < 0){
            transform.position = Vector3.MoveTowards(transform.position, ladderInfo.bottomPos.position, 2 * Time.deltaTime);
            pController.anim.SetBool("isClimbingUp", false);
            pController.anim.SetBool("isClimbingDown", true);
            pController.anim.speed = Mathf.Lerp(pController.anim.speed, 1.5f, .3f);
            if(Vector3.Distance(transform.position, ladderInfo.bottomPos.position) <= .1){
                LadderEnd();
            }
        }else{
            pController.anim.speed = Mathf.Lerp(pController.anim.speed, 0, .3f);
        }
    }

    public void LadderEnd(){
        rb.isKinematic = false;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
        pController.anim.Play("Idle");
        pController.anim.SetBool("isClimbing", false);
        pController.anim.SetBool("isClimbingUp", false);
        pController.anim.SetBool("isClimbingDown", false);
        pController.anim.speed = 1f;
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
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Traversing;
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

    public void EndShimy(){
        PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
        rb.isKinematic = false;
    }

    public bool CanGrabLedge(GameObject ledge){
        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.y = transform.position.y + 1;
        if(Physics.Raycast(origin, transform.forward, out hit, 5)){
            StartCoroutine(GrabLedge(ledge, hit.point));
            return true;
        }
        return false;
    }

    public IEnumerator GrabLedge(GameObject ledge, Vector3 hit){
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Traversing;
        rb.isKinematic = true;
        Vector3  topOfLedge = new Vector3(hit.x, ledge.transform.position.y - 1, hit.z);
        topOfLedge.z = topOfLedge.z + .5f;
        while(Vector3.Distance(transform.position, topOfLedge) > .5f){
            transform.position = Vector3.Lerp(transform.position, topOfLedge, .1f);
            Vector3 tp = topOfLedge;    
            tp.y = transform.position.y;

            if(!pController.ledge){
                DropLedge();
                yield break;
            }
            yield return new WaitForEndOfFrame();
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

    public void DropLedge(){
        rb.isKinematic = false;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
    }

    private void BetterJumpPhysics(){
        if (rb.velocity.y  < 0) 
            rb.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
        else if (rb.velocity.y  > 0 /*&& !Input.GetButton("Jump")*/)
            rb.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
    }
}
