using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private PlayerManager pManager;
    private PlayerController pController;

    [SerializeField]
    PlayerTargeting pTargeting;

    private Rigidbody rb;
    private Animator anim;
    private GameObject pCamera;

    private float fallMultiplyer = 10f;
    private float lowJumpMultiplyer = 3f;

    private void Start(){
        pManager = PlayerManager.instance;
        pController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        pCamera = Camera.main.gameObject;
    }

    private void FixedUpdate(){
        BetterJumpPhysics();
    }

    public void FreeMovement(Vector3 movement, float speed){
        if (pManager.currentState != PlayerManager.PlayerState.Attacking){
            if(pManager.isLockedOn && pManager.isBlocking){
                speed = speed/2.5f;
            }else if(!pController.CheckGrounded()){
                speed = speed / 1.5f;
            }

            // Vector3 dir = pCamera.transform.position - transform.position;
            // dir.y = 0;
            movement = pCamera.transform.TransformDirection(movement);
            movement.y = 0;

            rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);

            if(pController.CheckGrounded() && movement != Vector3.zero){
                if(!pManager.isLockedOn || !pManager.isBlocking){
                    float turnSpeed = Mathf.Max(Mathf.Abs(movement.x), Mathf.Abs(movement.z));
                    if(turnSpeed > .5f){
                        turnSpeed = 0.5f;
                    }
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
                }
            }
        }
    }

    public void AnimatePlayerWalking(Vector3 moveDir){
        if(pManager.isLockedOn && pManager.isBlocking){
            if(moveDir.z > 0.5f){
                moveDir.z = 0.5f;
            }
            anim.SetFloat("velocityY", Mathf.Lerp(anim.GetFloat("velocityY"), moveDir.z, .1f));
            anim.SetFloat("velocityX", Mathf.Lerp(anim.GetFloat("velocityX"), moveDir.x, .1f));
            LookAtTarget(pTargeting.currentTarget.transform);
        }else{
            anim.SetFloat("velocityY", Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z)));
        }
    }

    public void LookAtTarget(Transform target){
        Vector3 tp = target.position;
        tp.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(tp - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, .2f);
    }

    public void Jump(float jumpHeight){
        rb.velocity = new Vector3(0,jumpHeight, 0);
        anim.SetBool("isGrounded", false);
        anim.Play("Jump");
    }

    public void Evade(Vector3 dir = new Vector3()){
        if(pManager.isVulnerable){
            if(dir == Vector3.zero){
                if(pManager.isLockedOn){
                    Vector3 tp = pTargeting.currentTarget.transform.position;
                    tp.y = transform.position.y;

                    transform.LookAt(tp);
                }
                anim.Play("StepBack");
            }else{
                anim.Play("Roll");
            }
            
            pManager.StartCoroutine(pManager.Invulnerabe());
        }
    }


    private void BetterJumpPhysics(){
        if (rb.velocity.y  < 0) 
            rb.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
        else if (rb.velocity.y  > 0 /*&& !Input.GetButton("Jump")*/)
            rb.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
    }
}
